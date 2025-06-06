using ArpellaStores.Features.Authentication.Models;
using ArpellaStores.Features.Delivery_Tracking_Management.Models;

namespace ArpellaStores.Models;

public partial class Order
{
    public string Orderid { get; set; } = null!;

    public string? UserId { get; set; }
    public string? PhoneNumber { get; set; }

    public string? Status { get; set; }

    public decimal? Total { get; set; }

    public string? BuyerPin { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public virtual ICollection<Deliverytracking> Deliverytrackings { get; set; } = new List<Deliverytracking>();

    public virtual ICollection<Orderitem> Orderitems { get; set; } = new List<Orderitem>();

    public virtual User? User { get; set; }
}
