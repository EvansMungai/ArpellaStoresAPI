using System;
using System.Collections.Generic;

namespace ArpellaStores.Models;

public partial class Product
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public int? Price { get; set; }

    public string? Category { get; set; }

    public virtual Category? CategoryNavigation { get; set; }
}
