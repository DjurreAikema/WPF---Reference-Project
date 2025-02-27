using Microsoft.EntityFrameworkCore;
using WpfApp1.Data;
using WpfApp1.Shared.Classes;
using WpfApp1.Shared.Locking.V1;

namespace WpfApp1.Shared.DataAccess;

/// <summary>
/// Enhanced service for Snack data access with integrated locking
/// </summary>
public class SnackServiceV3 : IDisposable
{
    public bool SimulateFailures { get; set; } = false;
    public double FailureProbability { get; set; } = 0.3;
    public double FailureProbabilityOnLoad { get; set; } = 0.3;
    private static readonly Random RandomGenerator = new Random();

    private readonly ILockService _lockService;

    public SnackServiceV3(ILockService? lockService = null)
    {
        // Create default lock service if none provided
        _lockService = lockService ?? new SqliteLockService(
            null,
            new LockServiceOptions
            {
                DefaultLockDuration = TimeSpan.FromMinutes(30),
                EnableAuditLogging = true,
                CleanupInterval = TimeSpan.FromMinutes(5)
            });
    }

    private static SnackDbContextV2 CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<SnackDbContextV2>();
        optionsBuilder.UseSqlite("Data Source=SnacksV2.db");
        return new SnackDbContextV2(optionsBuilder.Options);
    }

    /// <summary>
    /// Gets the lock service used by this SnackService
    /// </summary>
    public ILockService LockService => _lockService;

    /// <summary>
    /// Gets all snacks from the database
    /// </summary>
    public async Task<List<SnackV2>> GetAllSnacksAsync()
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbabilityOnLoad)
            throw new Exception("Simulated database failure during GetAllSnacksAsync");

        await using var context = CreateDbContext();
        var snacks = await context.Snacks.ToListAsync();

        // Refresh lock states for all items
        foreach (var snack in snacks)
        {
            // We could do this more efficiently in bulk, but for simplicity we check individually
            var state = await _lockService.GetLockStateAsync(snack);
        }

        return snacks;
    }

    /// <summary>
    /// Adds a new snack to the database
    /// </summary>
    public async Task<SnackV2> AddSnackAsync(SnackV2 snack)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during AddSnackAsync");

        await using var context = CreateDbContext();
        context.Snacks.Add(snack);
        await context.SaveChangesAsync();
        return snack;
    }

    /// <summary>
    /// Updates a snack in the database, checking lock status first
    /// </summary>
    public async Task<SnackV2> UpdateSnackAsync(SnackV2 snack)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during UpdateSnackAsync");

        // Verify lock status
        var lockState = await _lockService.GetLockStateAsync(snack);
        if (lockState != LockState.LockedByMe && lockState != LockState.Unlocked)
            throw new InvalidOperationException($"Cannot update snack - current lock state: {lockState}");

        await using var context = CreateDbContext();
        context.Snacks.Update(snack);
        await context.SaveChangesAsync();

        // If item was updated successfully and auto-refresh is enabled, refresh the lock
        if (lockState == LockState.LockedByMe)
        {
            await _lockService.RefreshLockAsync(snack);
        }

        return snack;
    }

    /// <summary>
    /// Deletes a snack from the database, checking lock status first
    /// </summary>
    public async Task<SnackV2?> DeleteSnackAsync(int id)
    {
        if (SimulateFailures && RandomGenerator.NextDouble() < FailureProbability)
            throw new Exception("Simulated database failure during DeleteSnackAsync");

        await using var context = CreateDbContext();
        var snack = await context.Snacks.FindAsync(id);
        if (snack == null) return snack;

        // Verify lock status
        var lockState = await _lockService.GetLockStateAsync(snack);
        if (lockState != LockState.LockedByMe && lockState != LockState.Unlocked)
            throw new InvalidOperationException($"Cannot delete snack - current lock state: {lockState}");

        // Remove the item
        context.Snacks.Remove(snack);
        await context.SaveChangesAsync();

        // If the item was locked, release the lock
        if (lockState == LockState.LockedByMe)
        {
            await _lockService.ReleaseLockAsync(snack);
        }

        return snack;
    }

    /// <summary>
    /// Gets a single snack by ID
    /// </summary>
    public async Task<SnackV2?> GetSnackByIdAsync(int id)
    {
        await using var context = CreateDbContext();
        return await context.Snacks.FindAsync(id);
    }

    /// <summary>
    /// Gets all snacks locked by the current user
    /// </summary>
    public async Task<List<SnackV2>> GetMyLockedSnacksAsync()
    {
        return (await _lockService.GetMyLocksAsync<SnackV2>()).ToList();
    }

    /// <summary>
    /// Gets all locked snacks across all users
    /// </summary>
    public async Task<List<SnackV2>> GetAllLockedSnacksAsync()
    {
        return (await _lockService.GetAllLocksAsync<SnackV2>()).ToList();
    }

    /// <summary>
    /// Releases all locks held by the current user
    /// </summary>
    public async Task ReleaseAllMyLocksAsync()
    {
        var myLocks = await GetMyLockedSnacksAsync();
        foreach (var snack in myLocks)
        {
            await _lockService.ReleaseLockAsync(snack);
        }
    }

    /// <summary>
    /// Cleans up any expired locks in the database
    /// </summary>
    public async Task CleanupExpiredLocksAsync()
    {
        await _lockService.CleanupExpiredLocksAsync<SnackV2>();
    }

    public void Dispose()
    {
        (_lockService as IDisposable)?.Dispose();
    }
}