using System;
using System.Collections.Generic;

namespace ArpellaStores.Models;

public partial class Orderitem
{
    public string? OrderId { get; set; }

    public string? ProductId { get; set; }

    public int? Quantity { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
