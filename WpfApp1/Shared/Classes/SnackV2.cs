using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WpfApp1.Shared.Locking.V1;

namespace WpfApp1.Shared.Classes;

/// <summary>
/// Enhanced SnackV2 class with improved locking capabilities
/// </summary>
public class SnackV2 : ILockable
{
    // --- Internal
    [Key] public int? Id { get; set; }

    // --- Form
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public double Price { get; set; }
    [Required] public int Quantity { get; set; }

    // --- Locking - Enhanced
    public DateTime? LockedAt { get; set; }
    public string? LockedBy { get; set; }
    public DateTime? LockExpiresAt { get; set; }
    public string? LockReason { get; set; }

    [NotMapped]
    public LockState LockState => this.GetLockState(Environment.UserName);

    public SnackV2()
    {
    }

    public SnackV2(SnackV2 other)
    {
        Id = other.Id;
        Name = other.Name;
        Price = other.Price;
        Quantity = other.Quantity;
        LockedAt = other.LockedAt;
        LockedBy = other.LockedBy;
        LockExpiresAt = other.LockExpiresAt;
        LockReason = other.LockReason;
    }
}