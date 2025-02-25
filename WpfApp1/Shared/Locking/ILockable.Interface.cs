namespace WpfApp1.Shared.Locking;

public interface ILockable
{
    /// <summary>
    /// Indicates the current lock state for this item (Locked, SoftLocked, Unlocked).
    /// </summary>
    LockState LockState { get; set; }

    /// <summary>
    /// Name or ID of the user who locked the item. If null, item is SoftLocked.
    /// </summary>
    string? LockedBy { get; set; }
}