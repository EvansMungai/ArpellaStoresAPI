namespace ArpellaStores.Models;

public partial class Product
{
    public int Id { get; set; }
    public string InventoryId { get; set; } = null!;

    public string? Name { get; set; }

    public decimal Price { get; set; }

    public int? Category { get; set; }
    public int? PurchaseCap { get; set; }

    public decimal? PriceAfterDiscount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? DiscountQuantity { get; set; }

    public int? Subcategory { get; set; }

    public string? Barcodes { get; set; }

    public string? TaxCode { get; set; }

    public virtual Category? CategoryNavigation { get; set; }

    public virtual ICollection<Flashsale> Flashsales { get; set; } = new List<Flashsale>();

    public virtual ICollection<Orderitem> Orderitems { get; set; } = new List<Orderitem>();

    public virtual Subcategory? SubcategoryNavigation { get; set; }

    public virtual ICollection<Productimage> Productimages { get; set; } = new List<Productimage>();
    public Inventory IdNavigation { get; set; }
}
