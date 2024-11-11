using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArpellaStores.Models;

public partial class Category
{
    [Key]
    public string Id { get; set; } = null!;

    public string? CategoryName { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
}
