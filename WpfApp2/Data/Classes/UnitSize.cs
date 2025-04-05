using System.ComponentModel.DataAnnotations;

namespace WpfApp2.Data.Classes;

public class UnitSize
{
    [Key] public int? Id { get; set; }
    public int? SnackId { get; set; }

    public string Name { get; set; } = string.Empty;
    public double Price { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }

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