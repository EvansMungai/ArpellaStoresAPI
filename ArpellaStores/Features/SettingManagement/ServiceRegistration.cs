namespace ArpellaStores.Features.SettingManagement.Services;

public static class ServiceRegistration
{
    public static void RegisterApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ISettingService, SettingService>();
    }
}
