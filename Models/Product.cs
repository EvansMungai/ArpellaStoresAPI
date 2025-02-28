using System;
using System.Collections.Generic;

namespace ArpellaStores.Models;

public partial class Product
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public decimal Price { get; set; }

    public string? Category { get; set; }

    public decimal? PriceAfterDiscount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public decimal? TaxRate { get; set; }

    public int? DiscountQuantity { get; set; }

    public string? Subcategory { get; set; }

    public string? Barcodes { get; set; }

    public virtual Category? CategoryNavigation { get; set; }

    public virtual ICollection<Flashsale> Flashsales { get; set; } = new List<Flashsale>();

    public virtual Inventory IdNavigation { get; set; } = null!;

    public virtual ICollection<Orderitem> Orderitems { get; set; } = new List<Orderitem>();

    public virtual ICollection<Productimage> Productimages { get; set; } = new List<Productimage>();

    public virtual Subcategory? SubcategoryNavigation { get; set; }
}
