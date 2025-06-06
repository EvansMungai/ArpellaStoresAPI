using ArpellaStores.Features.Final_Price_Management.Services;

namespace ArpellaStores.Features.Final_Price_Management;

public static class FinalPriceManagementServices
{
    public static void RegisterApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ICouponService, CouponService>();
        serviceCollection.AddScoped<IDiscountService, DiscountService>();
        serviceCollection.AddScoped<IFlashsaleService, FlashsaleService>();
        serviceCollection.AddScoped<IFinalPriceService, FinalPriceService>();
    }
}
