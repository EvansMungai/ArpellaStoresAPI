using ArpellaStores.Services;

namespace ArpellaStores.Features.Payment_Management.Services;

public static class PaymentManagementServices
{
    public static void RegisterApplicationServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient<IMpesaService, MpesaService>();
        serviceCollection.AddTransient<IMpesaService, MpesaService>();
    }
}
