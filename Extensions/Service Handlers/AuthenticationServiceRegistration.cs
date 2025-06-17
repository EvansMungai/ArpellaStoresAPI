using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.Authentication.Models;
using Microsoft.AspNetCore.Identity;

namespace ArpellaStores.Extensions.Service_Handlers;

public static class AuthenticationServiceRegistration
{
    public static void ConfigureAuthenticationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ArpellaContext>().AddDefaultTokenProviders();
        serviceCollection.AddAuthentication();
        serviceCollection.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
            options.AddPolicy("CustomerPolicy", policy => policy.RequireRole("Customer"));
            options.AddPolicy("InventoryManagerPolicy", policy => policy.RequireRole("Inventory Manager", "Admin"));
            options.AddPolicy("OrderManager", policy => policy.RequireRole("Order Manager", "Admin"));
            options.AddPolicy("DeliveryGuy", policy => policy.RequireRole("DeliveryGuy", "Admin"));
            options.AddPolicy("Accountant", policy => policy.RequireRole("Accountant", "Admin"));
        });
    }
}
