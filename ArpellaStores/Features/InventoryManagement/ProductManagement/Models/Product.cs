using ArpellaStores.Features.FinalPriceManagement.Models;
using ArpellaStores.Features.OrderManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace ArpellaStores.Features.InventoryManagement.Models;

public partial class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Inventory Id is required.")]
    [StringLength(150, ErrorMessage = "Inventory Id must be at most 150 characters.")]
    public string InventoryId { get; set; } = null!;

    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(255, ErrorMessage = "Product name must be at most 255 characters.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Product price is required.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Category Id is required.")]
    public int? Category { get; set; }

    public int? PurchaseCap { get; set; }

    [Required(ErrorMessage = "Product price after discount is required.")]
    public decimal? PriceAfterDiscount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? DiscountQuantity { get; set; }

    public int? Subcategory { get; set; }

    public string? Barcodes { get; set; }

    public virtual Category? CategoryNavigation { get; set; }

    public virtual ICollection<Flashsale> Flashsales { get; set; } = new List<Flashsale>();

    public virtual ICollection<Orderitem> Orderitems { get; set; } = new List<Orderitem>();

    public virtual Subcategory? SubcategoryNavigation { get; set; }

    public virtual ICollection<Productimage> Productimages { get; set; } = new List<Productimage>();
    public Inventory IdNavigation { get; set; }
}
