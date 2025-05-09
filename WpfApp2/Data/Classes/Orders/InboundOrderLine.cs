using System.ComponentModel.DataAnnotations;
using WpfApp2.Data.Classes.Stamdata;

namespace WpfApp2.Data.Classes.Orders;

public class InboundOrderLine
{
    [Key] public int? Id { get; set; }
    public int InboundOrderId { get; set; }
    public int SnackId { get; set; }
    public int? UnitSizeId { get; set; }

    public int Quantity { get; set; }
    public double UnitPrice { get; set; }

    // --- Navigation properties
    public virtual InboundOrder? Order { get; set; }
    public virtual Snack? Snack { get; set; }
    public virtual UnitSize? UnitSize { get; set; }
}