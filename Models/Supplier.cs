using System;
using System.Collections.Generic;

namespace ArpellaStores.Models;

public partial class Supplier
{
    public int Id { get; set; }

    public string? SupplierName { get; set; }

    public string? KraPin { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}
