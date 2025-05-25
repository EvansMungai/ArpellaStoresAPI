using ArpellaStores.Services;

namespace ArpellaStores.Features.Delivery_Tracking_Management.Services;

public static class DeliveryTrackingManagementServices
{
    public static void RegisterApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IDeliveryTrackingService, DeliveryTrackingService>();
    }
}
