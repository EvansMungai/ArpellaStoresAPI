using System;
using System.Collections.Generic;

namespace ArpellaStores.Models;

public partial class Productimage
{
    public int ImageId { get; set; }

    public string? ProductId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool? IsPrimary { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Product? Product { get; set; }
}
