using ArpellaStores.Features.Authentication.Services.Authentication;
using ArpellaStores.Services;

namespace ArpellaStores.Features.Authentication;

public static class AuthenticationManagementServices
{
    public static void RegisterApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IAuthenticationService, AuthenticationService>();
        serviceCollection.AddScoped<IUserManagementService, UserManagementService>();
    }
}
