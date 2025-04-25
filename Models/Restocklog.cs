using System;
using System.Collections.Generic;

namespace ArpellaStores.Models;

public partial class Restocklog
{
    public int LogId { get; set; }

    public string? ProductId { get; set; }

    public int? RestockQuantity { get; set; }

    public DateTime? RestockDate { get; set; }

    public int? SupplierId { get; set; }

    public string? InvoiceNumber { get; set; }
}
