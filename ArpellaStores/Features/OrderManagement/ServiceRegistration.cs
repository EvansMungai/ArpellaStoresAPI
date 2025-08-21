namespace ArpellaStores.Features.OrderManagement.Services;

public static class ServiceRegistration
{
    public static void RegisterApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IOrderRepository, OrderRepository>();
        serviceCollection.AddScoped<IOrderPaymentService, OrderPaymentService>();
        serviceCollection.AddScoped<IOrderHelper, OrderHelper>();
        serviceCollection.AddScoped<IOrderCacheService, OrderCacheService>();
        serviceCollection.AddScoped<IOrderFinalizerService, OrderFinalizerService>();
        serviceCollection.AddScoped<IOrderService, OrderService>();
    }
}
