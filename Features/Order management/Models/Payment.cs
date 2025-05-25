using System;
using System.Collections.Generic;

namespace ArpellaStores.Models;

public partial class Payment
{
    public string? Orderid { get; set; }

    public string? Status { get; set; }

    public string? TransactionId { get; set; }

    public int PaymentId { get; set; }

    public virtual Order? Order { get; set; }
}
