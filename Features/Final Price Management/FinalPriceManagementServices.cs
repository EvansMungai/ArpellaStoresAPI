using ArpellaStores.Services;

namespace ArpellaStores.Features.Final_Price_Management;

public static class FinalPriceManagementServices
{
    public static void RegisterApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<ICouponService, CouponService>();
        serviceCollection.AddTransient<IDiscountService, DiscountService>();
        serviceCollection.AddTransient<IFlashsaleService, FlashsaleService>();
        serviceCollection.AddTransient<IFinalPriceService, FinalPriceService>();
    }
}
