using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace WpfApp2.Shared.Classes;

public class Warehouse
{
    [Key] public int? Id { get; set; }
    public int? CountryId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string HouseNumber { get; set; } = string.Empty;

    // --- Many
    public ObservableCollection<Snack>? Snacks { get; set; }

    // --- Constructors
    public Warehouse()
    {
    }

    public Warehouse(Warehouse other)
    {
        Id = other.Id;
        CountryId = other.CountryId;

        Name = other.Name;
        City = other.City;
        ZipCode = other.ZipCode;
        Address = other.Address;
        HouseNumber = other.HouseNumber;

        Snacks = other.Snacks;
    }
}