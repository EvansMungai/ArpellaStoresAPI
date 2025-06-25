namespace ArpellaStores.Features.Authentication.Services;

public static class ServiceRegistration
{
    public static void RegisterApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUserManagementService, UserManagementService>();
        serviceCollection.AddScoped<IAuthenticationService, AuthenticationService>();
    }
}
