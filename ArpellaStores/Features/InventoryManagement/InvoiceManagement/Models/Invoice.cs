using System.ComponentModel.DataAnnotations;

namespace ArpellaStores.Features.InventoryManagement.Models;

public partial class Invoice
{
    [Required(ErrorMessage = "Invoice Id is required.")]
    [StringLength(30, ErrorMessage = "Invoice Id must be at most 30 characters.")]
    public string InvoiceId { get; set; } = null!;

    [Required(ErrorMessage = "Supplier Id is required.")]
    public int? SupplierId { get; set; }

    [Required(ErrorMessage ="Total Amount is required.")]
    public decimal? TotalAmount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Supplier? Supplier { get; set; }
    public virtual ICollection<Restocklog> Restocklogs { get; set; } = new List<Restocklog>();
    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}
