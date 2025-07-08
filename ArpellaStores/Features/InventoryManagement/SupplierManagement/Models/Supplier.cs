using System.ComponentModel.DataAnnotations;

namespace ArpellaStores.Features.InventoryManagement.Models;

public partial class Supplier
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Supplier Name is required.")]
    [StringLength(50, ErrorMessage = " Supplier Name must be at most 30 characters.")]
    public string? SupplierName { get; set; }

    [Required(ErrorMessage = "KRA pin is required.")]
    [StringLength(30, ErrorMessage ="KRA pin must be at most 30 characters.")]
    public string? KraPin { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public virtual ICollection<Invoice> Invoices { get; set; }
    public virtual ICollection<Restocklog> Restocklogs { get; set; } = new List<Restocklog>();
}
