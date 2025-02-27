namespace WpfApp1.Shared.Locking.V1;

/// <summary>
/// Enhanced interface for lockable entities with additional metadata
/// </summary>
public interface ILockable
{
    /// <summary>
    /// Unique identifier for the lockable entity
    /// </summary>
    int? Id { get; }

    /// <summary>
    /// When the item was locked (UTC)
    /// </summary>
    DateTime? LockedAt { get; set; }

    /// <summary>
    /// Who locked the item
    /// </summary>
    string? LockedBy { get; set; }

    /// <summary>
    /// Optional lock expiration time (UTC)
    /// </summary>
    DateTime? LockExpiresAt { get; set; }

    /// <summary>
    /// Optional reason for locking
    /// </summary>
    string? LockReason { get; set; }

    /// <summary>
    /// Computed property for current lock state
    /// </summary>
    LockState LockState { get; }
}