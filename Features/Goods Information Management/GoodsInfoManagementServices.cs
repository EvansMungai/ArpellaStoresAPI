using ArpellaStores.Services;

namespace ArpellaStores.Features.Goods_Information_Management.Services;

public static class GoodsInfoManagementServices
{
    public static void RegisterApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IGoodsInformationService, GoodsInformationService>();
    }
}
