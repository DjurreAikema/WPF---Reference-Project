namespace WpfApp1.Shared.Locking.V1;

/// <summary>
/// Configuration options for the lock service
/// </summary>
public class LockServiceOptions
{
    /// <summary>
    /// Default duration for locks
    /// </summary>
    public TimeSpan DefaultLockDuration { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Max duration allowed for locks
    /// </summary>
    public TimeSpan MaxLockDuration { get; set; } = TimeSpan.FromHours(8);

    /// <summary>
    /// How often to run the expired lock cleanup task
    /// </summary>
    public TimeSpan CleanupInterval { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Whether to enable audit logging
    /// </summary>
    public bool EnableAuditLogging { get; set; } = true;

    /// <summary>
    /// Whether to automatically refresh locks when the entity is updated
    /// </summary>
    public bool AutoRefreshOnUpdate { get; set; } = true;
}