using System.ComponentModel.DataAnnotations;

namespace WpfApp1.Shared.Classes;

public class Snack
{
    [Key] public int? Id { get; set; }
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public double Price { get; set; }
    [Required] public int Quantity { get; set; }

    public SnackSub? SnackSub { get; set; }

    public Snack()
    {
    }

    public Snack(Snack other)
    {
        Id = other.Id;
        Name = other.Name;
        Price = other.Price;
        Quantity = other.Quantity;
    }
}