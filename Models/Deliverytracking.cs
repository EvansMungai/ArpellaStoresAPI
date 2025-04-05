using System;
using System.Collections.Generic;

namespace ArpellaStores.Models;

public partial class Deliverytracking
{
    public int DeliveryId { get; set; }

    public string OrderId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string? DeliveryAgent { get; set; }

    public string? Status { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual User UsernameNavigation { get; set; } = null!;
}
