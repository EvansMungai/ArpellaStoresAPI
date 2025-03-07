﻿using System;
using System.Collections.Generic;

namespace ArpellaStores.Models;

public partial class Inventory
{
    public string ProductId { get; set; } = null!;

    public int? StockQuantity { get; set; }

    public int? StockThreshold { get; set; }

    public decimal? StockPrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Product? Product { get; set; }
}
