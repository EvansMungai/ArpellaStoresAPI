using System;
using System.Collections.Generic;

namespace ArpellaStores.Models;

public partial class Order
{
    public string OrderId { get; set; } = null!;

    public string? UserId { get; set; }

    public string? Status { get; set; }

    public decimal? Total { get; set; }

    public virtual User? User { get; set; }
    public virtual ICollection<Orderitem> Orderitems { get; set; } = new List<Orderitem>();
}
