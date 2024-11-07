using System;
using System.Collections.Generic;

namespace ArpellaStores.Models;

public partial class Subcategory
{
    public string Id { get; set; } = null!;

    public string? SubcategoryName { get; set; }

    public string? CategoryId { get; set; }

    public virtual Category? Category { get; set; }
}
