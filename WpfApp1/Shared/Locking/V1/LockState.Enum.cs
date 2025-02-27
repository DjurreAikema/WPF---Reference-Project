namespace WpfApp1.Shared.Locking.V1;

/// <summary>
/// Enhanced lock state enumeration
/// </summary>
public enum LockState
{
    /// <summary>
    /// The item is not locked and available for editing
    /// </summary>
    Unlocked,

    /// <summary>
    /// The item is locked by the current user
    /// </summary>
    LockedByMe,

    /// <summary>
    /// The item is locked by another user
    /// </summary>
    LockedByOther,

    /// <summary>
    /// The lock has expired but hasn't been released
    /// </summary>
    LockExpired
}