using ArpellaStores.Features.Authentication.Services;
using ArpellaStores.Features.Authentication.Services.Authentication;

namespace ArpellaStores.Features.Authentication;

public static class AuthenticationManagementServices
{
    public static void RegisterApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IAuthenticationService, AuthenticationService>();
        serviceCollection.AddScoped<IUserManagementService, UserManagementService>();
    }
}
