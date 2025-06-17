using ArpellaStores.Features.FinalPriceManagement.Services;

namespace ArpellaStores.Features.FinalPriceManagement;

public static class ServiceRegistration
{
    public static void RegisterApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ICouponService, CouponService>();
        serviceCollection.AddScoped<IDiscountService, DiscountService>();
        serviceCollection.AddScoped<IFlashsaleService, FlashsaleService>();
        serviceCollection.AddScoped<IFinalPriceService, FinalPriceService>();
    }
}
