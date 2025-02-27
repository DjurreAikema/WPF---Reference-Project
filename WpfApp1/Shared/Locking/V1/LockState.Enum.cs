namespace WpfApp1.Shared.Locking.V1;

public enum LockState
{
    /// <summary>
    /// The item is locked by another user.  (User A cannot claim it if it’s locked by User B.)
    /// </summary>
    Locked,

    /// <summary>
    /// The item is not locked by any user; so it’s available for the user to claim (become Unlocked).
    /// </summary>
    SoftLocked,

    /// <summary>
    /// The item is locked by the **current** user. For other users, it appears locked.
    /// </summary>
    Unlocked
}