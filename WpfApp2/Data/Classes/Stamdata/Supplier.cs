using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using WpfApp2.Data.Classes.Orders;

namespace WpfApp2.Data.Classes.Stamdata;

public class Supplier
{
    [Key] public int? Id { get; set; }
    public int? CountryId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    // --- Navigation property
    public virtual Country? Country { get; set; }
    public virtual ObservableCollection<InboundOrder>? Orders { get; set; }
}