using ArpellaStores.Features.Payment_Management.Common;
using ArpellaStores.Features.Payment_Management.Services;
using ArpellaStores.Services;

namespace ArpellaStores.Features.Payment_Management.Services;

public static class PaymentManagementServices
{
    public static void RegisterApplicationServices(IServiceCollection serviceCollection)
    {
        string environmentUri = SystemEnvironmentUrl.Production;
        serviceCollection.AddHttpClient<MpesaApiService>(c =>
        {
            c.BaseAddress = new Uri(environmentUri);
        });
        serviceCollection.AddSingleton<IMpesaApiService, MpesaApiService>();
    }
}
