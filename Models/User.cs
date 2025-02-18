using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ArpellaStores.Models;

public partial class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime LastLoginTime { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
