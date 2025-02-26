using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ArpellaStores.Models;

public partial class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime LastLoginTime { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
