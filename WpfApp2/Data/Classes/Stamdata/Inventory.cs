using System.ComponentModel.DataAnnotations;

namespace WpfApp2.Data.Classes.Stamdata;

public class Inventory
{
    [Key] public int? Id { get; set; }
    public int SnackId { get; set; }
    public int? WarehouseId { get; set; }
    public int? UnitSizeId { get; set; }

    public int Quantity { get; set; }

    // --- Navigation properties
    public virtual Snack Snack { get; set; } = null!;
    public virtual Warehouse Warehouse { get; set; } = null!;
    public virtual UnitSize? UnitSize { get; set; }

    // --- Constructors
    public Inventory()
    {
    }

    public Inventory(Inventory other)
    {
        Id = other.Id;
        SnackId = other.SnackId;
        WarehouseId = other.WarehouseId;
        UnitSizeId = other.UnitSizeId;
        Quantity = other.Quantity;
    }
}