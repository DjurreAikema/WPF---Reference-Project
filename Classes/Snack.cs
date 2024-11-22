using System.ComponentModel.DataAnnotations;

namespace WpfApp1.Classes;

public class Snack
{
    [Key] public int? Id { get; set; }
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public double Price { get; set; }
    [Required] public int Quantity { get; set; }
}