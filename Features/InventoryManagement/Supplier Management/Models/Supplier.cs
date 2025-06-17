namespace ArpellaStores.Features.InventoryManagement.Models;

public partial class Supplier
{
    public int Id { get; set; }

    public string? SupplierName { get; set; }

    public string? KraPin { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public virtual ICollection<Invoice> Invoices { get; set; }
    public virtual ICollection<Restocklog> Restocklogs { get; set; } = new List<Restocklog>();
}
