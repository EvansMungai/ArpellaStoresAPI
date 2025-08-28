namespace ArpellaStores.Features.SmsManagement.Services;

public static class ServiceRegistration
{
    public static void RegisterApplicationServices(IServiceCollection services)
    {
        services.AddHttpClient<ISmsService, SmsService>();
        services.AddScoped<ISmsTemplateRepository, SmsTemplateRepository>();
        services.AddScoped<ISmsTemplateService, SmsTemplateService>();
    }
}
