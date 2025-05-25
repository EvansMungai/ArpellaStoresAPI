using System;
using System.Collections.Generic;

namespace ArpellaStores.Models;

public partial class Goodsinfo
{
    public string ItemCode { get; set; } = null!;

    public string? ItemDescription { get; set; }

    public string? UnitMeasure { get; set; }

    public decimal? TaxRate { get; set; }
}
