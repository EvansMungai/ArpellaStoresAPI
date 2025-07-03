using System.ComponentModel.DataAnnotations;

namespace ArpellaStores.Features.InventoryManagement.Models;

public partial class Inventory
{
    public int InventoryId { get; set; }

    [Required(ErrorMessage = "Product Id is required.")]
    [StringLength(30, ErrorMessage =" Proudct id must be at most 30 characters.")]
    public string ProductId { get; set; }

    [Required(ErrorMessage = "Stock quantity is required.")]
    public int? StockQuantity { get; set; }

    [Required(ErrorMessage = "Stoc threshold is required.")]
    public int? StockThreshold { get; set; }

    [Required(ErrorMessage = "Stock price is required.")]
    public decimal? StockPrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [Required(ErrorMessage = "Supplier Id is required")]
    public int? SupplierId { get; set; }

    public virtual Supplier? Supplier { get; set; }
    public ICollection<Product> Products { get; set; }
}
