using System.ComponentModel.DataAnnotations;

namespace WpfApp2.Shared.Classes;

public class UnitSize
{
    [Key] public int? Id { get; set; }

    public double Price { get; set; }
    public int Quantity { get; set; }

    // --- Constructors
    public UnitSize()
    {
    }

    public UnitSize(UnitSize other)
    {
        Id = other.Id;

        Price = other.Price;
        Quantity = other.Quantity;
    }
}