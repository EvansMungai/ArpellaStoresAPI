using ArpellaStores.Features.Delivery_Tracking_Management.Models;
using ArpellaStores.Models;
using Microsoft.AspNetCore.Identity;

namespace ArpellaStores.Features.Authentication.Models;

public partial class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime LastLoginTime { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Deliverytracking> Deliverytrackings { get; set; } = new List<Deliverytracking>();
}
