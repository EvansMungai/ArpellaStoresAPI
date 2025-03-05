using System;
using System.Collections.Generic;

namespace ArpellaStores.Models;

public partial class Subcategory
{
    public int Id { get; set; } 

    public string? SubcategoryName { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
