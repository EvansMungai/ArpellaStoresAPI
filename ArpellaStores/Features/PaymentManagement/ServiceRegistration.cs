using ArpellaStores.Features.PaymentManagement.Models;

namespace ArpellaStores.Features.PaymentManagement.Services;

public static class ServiceRegistration
{
    public static void RegisterApplicationServices(IServiceCollection serviceCollection)
    {
        string environmentUri = SystemEnvironmentUrl.Production;
        serviceCollection.AddHttpClient<MpesaApiService>(c =>
        {
            c.BaseAddress = new Uri(environmentUri);
        });
        serviceCollection.AddScoped<IMpesaApiService, MpesaApiService>();
        serviceCollection.AddScoped<IMpesaCallbackHandler, MpesaCallbackHandler>();
        serviceCollection.AddScoped<IPaymentResultHelper, PaymentResultHelper>();
    }
}
