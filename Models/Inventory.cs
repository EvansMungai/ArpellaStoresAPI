using System;
using System.Collections.Generic;

namespace ArpellaStores.Models;

public partial class Inventory
{
    public int InventoryId { get; set; }
    public string ProductId { get; set; }

    public int? StockQuantity { get; set; }

    public int? StockThreshold { get; set; }

    public decimal? StockPrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? SupplierId { get; set; }

    public virtual Supplier? Supplier { get; set; }
    public ICollection<Product> Products { get; set; }
}
