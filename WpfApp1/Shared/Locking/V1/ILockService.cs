namespace WpfApp1.Shared.Locking.V1;

/// <summary>
/// Service interface for lock management
/// </summary>
public interface ILockService
{
    /// <summary>
    /// Acquires a lock on an entity for the current user
    /// </summary>
    Task<LockResult> AcquireLockAsync<T>(T entity, string reason = null, TimeSpan? lockDuration = null) where T : class, ILockable;

    /// <summary>
    /// Releases a lock on an entity
    /// </summary>
    Task<LockResult> ReleaseLockAsync<T>(T entity) where T : class, ILockable;

    /// <summary>
    /// Refreshes an existing lock, extending its duration
    /// </summary>
    Task<LockResult> RefreshLockAsync<T>(T entity, TimeSpan? additionalTime = null) where T : class, ILockable;

    /// <summary>
    /// Forcibly breaks another user's lock (for admin use)
    /// </summary>
    Task<LockResult> BreakLockAsync<T>(T entity, string adminReason = null) where T : class, ILockable;

    /// <summary>
    /// Gets the current lock status of an entity
    /// </summary>
    Task<LockState> GetLockStateAsync<T>(T entity) where T : class, ILockable;

    /// <summary>
    /// Gets entities locked by the current user
    /// </summary>
    Task<IEnumerable<T>> GetMyLocksAsync<T>() where T : class, ILockable;

    /// <summary>
    /// Gets all locked entities with lock information
    /// </summary>
    Task<IEnumerable<T>> GetAllLocksAsync<T>() where T : class, ILockable;

    /// <summary>
    /// Checks for expired locks and automatically releases them
    /// </summary>
    Task CleanupExpiredLocksAsync<T>() where T : class, ILockable;

    /// <summary>
    /// Observable of lock events for real-time updates
    /// </summary>
    IObservable<LockResult> LockEvents { get; }
}