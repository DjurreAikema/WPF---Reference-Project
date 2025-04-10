using System.ComponentModel.DataAnnotations;

namespace WpfApp2.Data.Classes;

public class InboundOrder
{
    [Key] public int? Id { get; set; }

    // --- Constructors
    public InboundOrder()
    {
    }

    public InboundOrder(InboundOrder other)
    {
        Id = other.Id;
    }
}