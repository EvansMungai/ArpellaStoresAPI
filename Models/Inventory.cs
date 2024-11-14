using System;
using System.Collections.Generic;

namespace ArpellaStores.Models;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public string? ProductId { get; set; }

    public int? StockQuantity { get; set; }

    public int? StockThreshold { get; set; }

    public virtual Product? Product { get; set; }
}
