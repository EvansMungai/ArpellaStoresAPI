using System;
using System.Collections.Generic;

namespace ArpellaStores.Models;

public partial class Category
{
    public int Id { get; set; }

    public string? CategoryName { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
}
