using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace WpfApp2.Data.Classes;

public class Snack
{
    [Key] public int? Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public double Price { get; set; }
    public int Quantity { get; set; } // Total across all warehouses
    public bool MultipleUnitSizes { get; set; }

    // --- Navigation properties
    public ObservableCollection<UnitSize>? UnitSizes { get; set; }
    public ObservableCollection<Inventory>? Inventories { get; set; }

    // --- Constructors
    public Snack()
    {
    }

    public Snack(Snack other)
    {
        Id = other.Id;

        Brand = other.Brand;
        Name = other.Name;
        Price = other.Price;
        Quantity = other.Quantity;
        MultipleUnitSizes = other.MultipleUnitSizes;

        UnitSizes = other.UnitSizes;
    }
}