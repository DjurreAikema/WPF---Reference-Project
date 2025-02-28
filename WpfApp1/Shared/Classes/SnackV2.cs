using System.ComponentModel.DataAnnotations;

namespace WpfApp1.Shared.Classes;

public class SnackV2
{
    // --- Internal
    [Key] public int? Id { get; set; }

    // --- Form
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public double Price { get; set; }
    [Required] public int Quantity { get; set; }

    public SnackV2()
    {
    }

    public SnackV2(SnackV2 other)
    {
        Id = other.Id;
        Name = other.Name;
        Price = other.Price;
        Quantity = other.Quantity;
    }
}