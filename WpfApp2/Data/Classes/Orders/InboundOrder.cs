using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using WpfApp2.Data.Classes.Stamdata;
using WpfApp2.Data.Enums;

namespace WpfApp2.Data.Classes.Orders;

public class InboundOrder
{
    [Key] public int? Id { get; set; }
    public int? SupplierId { get; set; }

    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public DateTime? ExpectedDeliveryDate { get; set; }
    public OrderStatusEnum Status { get; set; } = OrderStatusEnum.Draft;
    public string Notes { get; set; } = string.Empty;

    // --- Navigation properties
    public virtual Supplier? Supplier { get; set; }
    public virtual ObservableCollection<InboundOrderLine>? OrderLines { get; set; }
    public virtual ObservableCollection<InboundReceipt>? Receipts { get; set; }
}