using ArpellaStores.Data;
using ArpellaStores.Extensions.RouteHandlers;
using ArpellaStores.Extensions.ServiceHandlers;
using ArpellaStores.Features.PaymentManagement.Models;
using RouteBuilder = ArpellaStores.Extensions.RouteHandlers.RouteBuilder;

namespace ArpellaStores.Extensions;

public static class ServiceRegistration
{
    public static void RegisterServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        // Add API Explorer & Swagger for documentation
        serviceCollection.ConfigureSwaggerUIDocumentation();

        //Configure DBContext
        serviceCollection.RegisterDataServices(configuration);

        // Configure JSON serializer options
        serviceCollection.ConfigureJsonSerializerSettings();

        // Configure Authentication and authorization services
        serviceCollection.ConfigureAuthenticationServices();

        serviceCollection.AddMemoryCache();

        // Configure Cors
        serviceCollection.ConfigureCors();

        // Bind and configure MpesaConfig
        serviceCollection.Configure<MpesaConfig>(configuration.GetSection("MpesaConfig"));

        // Register application services
        serviceCollection.RegisterFeatureServices();

        // Register Route Handlers for the endpoint services.
        serviceCollection.RegisterHandlers();

        // Register application endpoints services
        serviceCollection.RegisterRouteRegistrars();

        // Register RouteBuilder
        serviceCollection.AddScoped<RouteBuilder>();
    }
}
