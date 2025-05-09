using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace WpfApp2.Data.Classes.Stamdata;

public class Warehouse
{
    [Key] public int? Id { get; set; }
    public int? CountryId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string HouseNumber { get; set; } = string.Empty;

    // --- Navigation properties
    public ObservableCollection<Inventory>? Inventories { get; set; }

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
    }
}