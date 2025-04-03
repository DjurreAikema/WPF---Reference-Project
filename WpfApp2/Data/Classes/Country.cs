using System.ComponentModel.DataAnnotations;

namespace WpfApp2.Data.Classes;

public class Country
{
    [Key] public int? Id { get; set; }

    public string Name { get; set; } = string.Empty;

    // --- Constructors
    public Country()
    {
    }

    public Country(Country other)
    {
        Id = other.Id;

        Name = other.Name;
    }
}