using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArpellaStores.Models;

public partial class Subcategory
{
    [Key]
    public string Id { get; set; } = null!;

    public string? SubcategoryName { get; set; }

    public string? CategoryId { get; set; }

    public virtual Category? Category { get; set; }
}
