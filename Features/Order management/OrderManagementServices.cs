using ArpellaStores.Services;

namespace ArpellaStores.Features.Order_management;

public static class OrderManagementServices
{
    public static void RegisterApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IOrderService, OrderService>();
    }
}
