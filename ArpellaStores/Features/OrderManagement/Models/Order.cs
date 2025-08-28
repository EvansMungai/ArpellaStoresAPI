using ArpellaStores.Features.Authentication.Models;
using ArpellaStores.Features.DeliveryTrackingManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace ArpellaStores.Features.OrderManagement.Models;

public partial class Order
{
    public string Orderid { get; set; } = null!;

    [Required(ErrorMessage = "Username is required.")]
    [StringLength(30, ErrorMessage = "Username must be at most 30 characters.")]
    public string? UserId { get; set; }

    [StringLength(30, ErrorMessage = "Phone number must be at most 30 characters.")]
    public string PhoneNumber { get; set; }

    public string? Status { get; set; }

    public decimal Total { get; set; }

    public string? BuyerPin { get; set; }

    [Required(ErrorMessage = "Latitude is required.")]
    public decimal? Latitude { get; set; }

    [Required(ErrorMessage = "Longitude is required.")]
    public decimal? Longitude { get; set; }
    public string? OrderPaymentType { get; set; }
    public string? OrderSource { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public virtual ICollection<Deliverytracking> Deliverytrackings { get; set; } = new List<Deliverytracking>();

    public virtual ICollection<Orderitem> Orderitems { get; set; } = new List<Orderitem>();

    public virtual User? User { get; set; }
}
