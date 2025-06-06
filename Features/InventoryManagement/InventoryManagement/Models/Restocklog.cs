namespace ArpellaStores.Features.InventoryManagement.Models;

public partial class Restocklog
{
    public int LogId { get; set; }
    public string? ProductId { get; set; }
    public int? RestockQuantity { get; set; }
    public DateTime? RestockDate { get; set; }
    public int? SupplierId { get; set; }
    public string? InvoiceNumber { get; set; }
    public virtual Invoice? InvoiceNumberNavigation { get; set; }
    public virtual Supplier? Supplier { get; set; }
}
