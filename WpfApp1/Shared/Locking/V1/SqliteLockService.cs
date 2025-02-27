using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Shared.Locking.V1;

/// <summary>
    /// Implementation of the lock service for SQLite databases
    /// </summary>
    public class SqliteLockService : ILockService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly LockServiceOptions _options;
        private readonly string _currentUser;
        private readonly Subject<LockResult> _lockEvents = new Subject<LockResult>();
        private System.Threading.Timer _cleanupTimer;

        public IObservable<LockResult> LockEvents => _lockEvents.AsObservable();

        public SqliteLockService(
            IServiceProvider serviceProvider,
            LockServiceOptions options = null,
            string currentUser = null)
        {
            _serviceProvider = serviceProvider;
            _options = options ?? new LockServiceOptions();
            _currentUser = currentUser ?? Environment.UserName;

            // Set up cleanup timer
            _cleanupTimer = new System.Threading.Timer(
                async _ => await CleanupExpiredLocksInternalAsync(),
                null,
                TimeSpan.Zero,
                _options.CleanupInterval);
        }

        public async Task<LockResult> AcquireLockAsync<T>(T entity, string reason = null, TimeSpan? lockDuration = null) where T : class, ILockable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id == null)
                return LockResult.Failed(entity, "Entity must be saved before it can be locked");

            using var dbContext = GetDbContext<T>();
            var dbEntity = await GetEntityWithLockingAsync<T>(dbContext, entity.Id.Value);

            if (dbEntity == null)
                return LockResult.Failed(entity, "Entity not found");

            // Check current lock state
            var currentState = dbEntity.GetLockState(_currentUser);

            if (currentState == LockState.LockedByOther)
                return LockResult.Failed(dbEntity, $"Entity is already locked by {dbEntity.LockedBy}");

            if (currentState == LockState.LockedByMe)
                return await RefreshLockAsync(dbEntity, lockDuration);

            // Apply the lock
            var actualDuration = lockDuration ?? _options.DefaultLockDuration;
            if (actualDuration > _options.MaxLockDuration)
                actualDuration = _options.MaxLockDuration;

            dbEntity.LockedAt = DateTime.UtcNow;
            dbEntity.LockedBy = _currentUser;
            dbEntity.LockExpiresAt = DateTime.UtcNow.Add(actualDuration);
            dbEntity.LockReason = reason;

            await UpdateLockInDatabaseAsync(dbContext, dbEntity);

            if (_options.EnableAuditLogging)
                await LogLockOperationAsync(dbEntity, "Acquire", $"Duration: {actualDuration}, Reason: {reason}");

            var result = LockResult.Succeeded(dbEntity, "Lock acquired successfully");
            _lockEvents.OnNext(result);
            return result;
        }

        public async Task<LockResult> ReleaseLockAsync<T>(T entity) where T : class, ILockable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id == null)
                return LockResult.Failed(entity, "Entity must be saved before it can be unlocked");

            using var dbContext = GetDbContext<T>();
            var dbEntity = await GetEntityWithLockingAsync<T>(dbContext, entity.Id.Value);

            if (dbEntity == null)
                return LockResult.Failed(entity, "Entity not found");

            // Check if entity is actually locked
            if (dbEntity.LockedBy == null)
                return LockResult.Succeeded(dbEntity, "Entity was not locked");

            // Check if the current user is the one who locked it
            var currentState = dbEntity.GetLockState(_currentUser);
            if (currentState == LockState.LockedByOther)
                return LockResult.Failed(dbEntity, $"Cannot release a lock owned by another user ({dbEntity.LockedBy})");

            // Release the lock
            dbEntity.LockedAt = null;
            dbEntity.LockedBy = null;
            dbEntity.LockExpiresAt = null;
            dbEntity.LockReason = null;

            await UpdateLockInDatabaseAsync(dbContext, dbEntity);

            if (_options.EnableAuditLogging)
                await LogLockOperationAsync(dbEntity, "Release", "Lock released by owner");

            var result = LockResult.Succeeded(dbEntity, "Lock released successfully");
            _lockEvents.OnNext(result);
            return result;
        }

        public async Task<LockResult> RefreshLockAsync<T>(T entity, TimeSpan? additionalTime = null) where T : class, ILockable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id == null)
                return LockResult.Failed(entity, "Entity must be saved before its lock can be refreshed");

            using var dbContext = GetDbContext<T>();
            var dbEntity = await GetEntityWithLockingAsync<T>(dbContext, entity.Id.Value);

            if (dbEntity == null)
                return LockResult.Failed(entity, "Entity not found");

            // Check if entity is actually locked by the current user
            var currentState = dbEntity.GetLockState(_currentUser);
            if (currentState != LockState.LockedByMe)
                return LockResult.Failed(dbEntity, "Cannot refresh a lock that is not owned by the current user");

            // Calculate new expiration time
            var actualAdditionalTime = additionalTime ?? _options.DefaultLockDuration;
            var newExpiration = DateTime.UtcNow.Add(actualAdditionalTime);

            // If new expiration would exceed max duration, cap it
            var maxExpiration = dbEntity.LockedAt.Value.Add(_options.MaxLockDuration);
            if (newExpiration > maxExpiration)
                newExpiration = maxExpiration;

            dbEntity.LockExpiresAt = newExpiration;

            await UpdateLockInDatabaseAsync(dbContext, dbEntity);

            if (_options.EnableAuditLogging)
                await LogLockOperationAsync(dbEntity, "Refresh", $"Extended by {actualAdditionalTime}");

            var result = LockResult.Succeeded(dbEntity, "Lock refreshed successfully");
            _lockEvents.OnNext(result);
            return result;
        }

        public async Task<LockResult> BreakLockAsync<T>(T entity, string adminReason = null) where T : class, ILockable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id == null)
                return LockResult.Failed(entity, "Entity must be saved before its lock can be broken");

            using var dbContext = GetDbContext<T>();
            var dbEntity = await GetEntityWithLockingAsync<T>(dbContext, entity.Id.Value);

            if (dbEntity == null)
                return LockResult.Failed(entity, "Entity not found");

            // Don't need to break if not locked
            if (dbEntity.LockedBy == null)
                return LockResult.Succeeded(dbEntity, "Entity was not locked");

            // Record who had the lock before breaking it
            var previousOwner = dbEntity.LockedBy;

            // Break the lock
            dbEntity.LockedAt = null;
            dbEntity.LockedBy = null;
            dbEntity.LockExpiresAt = null;
            dbEntity.LockReason = null;

            await UpdateLockInDatabaseAsync(dbContext, dbEntity);

            if (_options.EnableAuditLogging)
            {
                await LogLockOperationAsync(
                    dbEntity,
                    "Break",
                    $"Lock forcibly broken by {_currentUser}. Previous owner: {previousOwner}. Reason: {adminReason}");
            }

            var result = LockResult.Succeeded(dbEntity, $"Lock broken successfully. Previous owner: {previousOwner}");
            _lockEvents.OnNext(result);
            return result;
        }

        public async Task<LockState> GetLockStateAsync<T>(T entity) where T : class, ILockable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id == null)
                return LockState.Unlocked; // Unsaved entities are always considered unlocked

            using var dbContext = GetDbContext<T>();
            var dbEntity = await GetEntityWithLockingAsync<T>(dbContext, entity.Id.Value);

            if (dbEntity == null)
                return LockState.Unlocked; // Entity not found, consider it unlocked

            return dbEntity.GetLockState(_currentUser);
        }

        public async Task<IEnumerable<T>> GetMyLocksAsync<T>() where T : class, ILockable
        {
            using var dbContext = GetDbContext<T>();
            var entitySet = dbContext.Set<T>();

            return await entitySet
                .Where(e => e.LockedBy == _currentUser)
                .ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllLocksAsync<T>() where T : class, ILockable
        {
            using var dbContext = GetDbContext<T>();
            var entitySet = dbContext.Set<T>();

            return await entitySet
                .Where(e => e.LockedBy != null)
                .ToListAsync();
        }

        public async Task CleanupExpiredLocksAsync<T>() where T : class, ILockable
        {
            using var dbContext = GetDbContext<T>();
            var entitySet = dbContext.Set<T>();
            var now = DateTime.UtcNow;

            var expiredLocks = await entitySet
                .Where(e => e.LockExpiresAt != null && e.LockExpiresAt < now)
                .ToListAsync();

            if (!expiredLocks.Any())
                return;

            foreach (var entity in expiredLocks)
            {
                // Record who had the lock before clearing it
                var previousOwner = entity.LockedBy;

                // Clear the lock
                entity.LockedAt = null;
                entity.LockedBy = null;
                entity.LockExpiresAt = null;
                entity.LockReason = null;

                if (_options.EnableAuditLogging)
                {
                    await LogLockOperationAsync(
                        entity,
                        "Expire",
                        $"Lock automatically expired. Previous owner: {previousOwner}");
                }

                var result = LockResult.Succeeded(entity, "Lock expired and was automatically released");
                _lockEvents.OnNext(result);
            }

            await dbContext.SaveChangesAsync();
        }

        private async Task CleanupExpiredLocksInternalAsync()
        {
            try
            {
                // You'll need to add specific implementations for each entity type that uses locking
                await CleanupExpiredLocksAsync<SnackV2>();
                // Add other entity types as needed
            }
            catch (Exception ex)
            {
                // Log the error but don't throw to prevent timer interruption
                Console.WriteLine($"Error in lock cleanup: {ex.Message}");
            }
        }

        private async Task<T> GetEntityWithLockingAsync<T>(DbContext dbContext, int id) where T : class, ILockable
        {
            return await dbContext.Set<T>().FindAsync(id);
        }

        private async Task UpdateLockInDatabaseAsync<T>(DbContext dbContext, T entity) where T : class, ILockable
        {
            dbContext.Attach(entity);

            // Only update locking fields
            dbContext.Entry(entity).Property(nameof(ILockable.LockedAt)).IsModified = true;
            dbContext.Entry(entity).Property(nameof(ILockable.LockedBy)).IsModified = true;
            dbContext.Entry(entity).Property(nameof(ILockable.LockExpiresAt)).IsModified = true;
            dbContext.Entry(entity).Property(nameof(ILockable.LockReason)).IsModified = true;

            await dbContext.SaveChangesAsync();
        }

        private async Task LogLockOperationAsync<T>(T entity, string operation, string details) where T : class, ILockable
        {
            // In a real implementation, you would log to a database table or logging system
            // This is a placeholder implementation
            Console.WriteLine(
                $"LOCK AUDIT: {DateTime.UtcNow} - {typeof(T).Name} ID {entity.Id} - {operation} by {_currentUser} - {details}");

            await Task.CompletedTask;
        }

        private DbContext GetDbContext<T>() where T : class, ILockable
        {
            // This method should be adapted to your application's DI setup
            // Here's a simple implementation that assumes SnackV2 is the only lockable entity

            if (typeof(T) == typeof(SnackV2))
            {
                var optionsBuilder = new DbContextOptionsBuilder<Data.SnackDbContextV2>();
                optionsBuilder.UseSqlite("Data Source=SnacksV2.db");
                return new Data.SnackDbContextV2(optionsBuilder.Options);
            }

            throw new NotSupportedException($"Entity type {typeof(T).Name} is not supported by the lock service");
        }

        public void Dispose()
        {
            _cleanupTimer?.Dispose();
        }
    }