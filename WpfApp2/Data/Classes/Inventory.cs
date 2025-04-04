using System.ComponentModel.DataAnnotations;

namespace WpfApp2.Data.Classes;

public class Inventory
{
    [Key] public int? Id { get; set; }
    public int SnackId { get; set; }
    public int WarehouseId { get; set; }

    public int Quantity { get; set; }

    // --- Navigation properties
    public Snack Snack { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;

    // --- Constructors
    public Inventory()
    {
    }

    public Inventory(Inventory other)
    {
        Id = other.Id;
        SnackId = other.SnackId;
        WarehouseId = other.WarehouseId;
        Quantity = other.Quantity;
    }
}