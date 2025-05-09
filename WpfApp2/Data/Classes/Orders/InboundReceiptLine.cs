using System.ComponentModel.DataAnnotations;

namespace WpfApp2.Data.Classes.Orders;

public class InboundReceiptLine
{
    [Key] public int? Id { get; set; }
    public int InboundReceiptId { get; set; }
    public int InboundOrderLineId { get; set; }

    public int QuantityReceived { get; set; }
    public string Notes { get; set; } = string.Empty;

    // --- Navigation properties
    public virtual InboundReceipt? Receipt { get; set; }
    public virtual InboundOrderLine? OrderLine { get; set; }
}