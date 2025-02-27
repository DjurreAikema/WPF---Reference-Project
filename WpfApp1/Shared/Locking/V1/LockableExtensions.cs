namespace WpfApp1.Shared.Locking.V1;

/// <summary>
/// Extension class for ILockable to provide additional functionality
/// </summary>
public static class LockableExtensions
{
    /// <summary>
    /// Calculates the current lock state based on lock properties and current user
    /// </summary>
    public static LockState GetLockState(this ILockable lockable, string currentUser)
    {
        if (lockable.LockedAt == null || lockable.LockedBy == null)
            return LockState.Unlocked;

        // Check if lock has expired
        if (lockable.LockExpiresAt.HasValue && lockable.LockExpiresAt.Value < DateTime.UtcNow)
            return LockState.LockExpired;

        // Check if locked by current user
        if (lockable.LockedBy.Equals(currentUser, StringComparison.OrdinalIgnoreCase))
            return LockState.LockedByMe;

        // Locked by someone else
        return LockState.LockedByOther;
    }
}