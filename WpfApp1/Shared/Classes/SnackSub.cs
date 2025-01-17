using System.ComponentModel.DataAnnotations;

namespace WpfApp1.Shared.Classes;

public class SnackSub
{
    [Key] public int? Id { get; set; }
    [Required] public string Description { get; set; } = string.Empty;
}