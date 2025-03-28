using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace WpfApp2.Shared.Classes;

public class Snack
{
    [Key] public int? Id { get; set; }
    public int? WarehouseId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public double Price { get; set; }
    public int Quantity { get; set; }
    public bool MultipleUnitSizes { get; set; }

    // --- Many
    public ObservableCollection<UnitSize>? UnitSize { get; set; }

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

        UnitSize = other.UnitSize;
    }
}