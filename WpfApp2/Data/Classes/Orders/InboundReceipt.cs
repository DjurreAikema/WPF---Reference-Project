using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using WpfApp2.Data.Classes.Stamdata;

namespace WpfApp2.Data.Classes.Orders;

public class InboundReceipt
{
    [Key] public int? Id { get; set; }
    public int InboundOrderId { get; set; }
    public int WarehouseId { get; set; }

    public DateTime ReceiptDate { get; set; } = DateTime.Now;
    public string ReceivedBy { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    // --- Navigation properties
    public virtual InboundOrder? Order { get; set; }
    public virtual Warehouse? Warehouse { get; set; }
    public virtual ObservableCollection<InboundReceiptLine>? ReceiptLines { get; set; }
}