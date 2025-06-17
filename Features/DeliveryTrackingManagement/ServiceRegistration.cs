namespace ArpellaStores.Features.DeliveryTrackingManagement.Services;

public static class ServiceRegistration
{
    public static void RegisterApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IDeliveryTrackingService, DeliveryTrackingService>();
    }
}
