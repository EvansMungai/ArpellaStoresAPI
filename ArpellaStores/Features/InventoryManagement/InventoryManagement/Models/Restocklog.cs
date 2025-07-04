using System.ComponentModel.DataAnnotations;

namespace ArpellaStores.Features.InventoryManagement.Models;

public partial class Restocklog
{
    public int LogId { get; set; }

    [Required(ErrorMessage = "Product id is required.")]
    [StringLength(30, ErrorMessage = "Product id must be at most 30 characters.")]
    public string? ProductId { get; set; }

    [Required(ErrorMessage = "Restock Quantity is required.")]
    public int? RestockQuantity { get; set; }
    public DateTime? RestockDate { get; set; }

    [Required(ErrorMessage = "Supplier id is required.")]
    public int? SupplierId { get; set; }

    [Required(ErrorMessage = "Invoice number is required.")]
    [StringLength(30, ErrorMessage = "Invoice number must be at most 30 characters.")]
    public string? InvoiceNumber { get; set; }
    public virtual Invoice? InvoiceNumberNavigation { get; set; }
    public virtual Supplier? Supplier { get; set; }
}
