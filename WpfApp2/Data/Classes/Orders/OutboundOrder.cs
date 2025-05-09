using System.ComponentModel.DataAnnotations;

namespace WpfApp2.Data.Classes.Orders;

public class OutboundOrder
{
    [Key] public int? Id { get; set; }

    // --- Constructors
    public OutboundOrder()
    {
    }

    public OutboundOrder(OutboundOrder other)
    {
        Id = other.Id;
    }
}