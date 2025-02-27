namespace WpfApp1.Shared.Locking.V1;

/// <summary>
/// Audit record for lock operations
/// </summary>
public class LockAuditRecord
{
    public int Id { get; set; }
    public int EntityId { get; set; }
    public string EntityType { get; set; }
    public string Operation { get; set; }
    public string UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Details { get; set; }
}