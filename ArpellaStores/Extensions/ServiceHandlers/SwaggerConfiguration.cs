using Microsoft.OpenApi.Models;

namespace ArpellaStores.Extensions.ServiceHandlers;

public static class SwaggerConfiguration
{
    public static void ConfigureSwaggerUIDocumentation(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ARPELLA STORES API", Description = "Building an ecommerce store", Version = "v1" });
        });
    }
}
