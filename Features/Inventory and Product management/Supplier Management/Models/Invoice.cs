﻿using System;
using System.Collections.Generic;

namespace ArpellaStores.Models;

public partial class Invoice
{
    public string InvoiceId { get; set; } = null!;

    public int? SupplierId { get; set; }

    public decimal? TotalAmount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Supplier? Supplier { get; set; }
    public virtual ICollection<Restocklog> Restocklogs { get; set; } = new List<Restocklog>();
}
