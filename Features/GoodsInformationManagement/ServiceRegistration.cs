using ArpellaStores.Features.GoodsInformationManagement.Services;

namespace ArpellaStores.Features.Goods_Information_Management.Services;

public static class ServiceRegistration
{
    public static void RegisterApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IGoodsInformationService, GoodsInformationService>();
    }
}
