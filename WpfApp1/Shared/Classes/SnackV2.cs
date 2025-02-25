using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WpfApp1.Shared.Locking;

namespace WpfApp1.Shared.Classes;

public class SnackV2 : ILockable
{
    // --- Internal
    [Key] public int? Id { get; set; }

    // --- Form
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public double Price { get; set; }
    [Required] public int Quantity { get; set; }

    // --- Locking
    public DateTime? Locked { get; set; }
    public string? LockedBy { get; set; }

    [NotMapped] public LockState LockState { get; set; } = LockState.SoftLocked;

    public SnackV2()
    {
    }

    public SnackV2(SnackV2 other)
    {
        Id = other.Id;
        Name = other.Name;
        Price = other.Price;
        Quantity = other.Quantity;
        Locked = other.Locked;
        LockedBy = other.LockedBy;
    }
}