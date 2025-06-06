using System.Text.Json.Serialization;

namespace ArpellaStores.Extensions.Service_Handlers;

public static class JSONSerializer
{
    public static void ConfigureJsonSerializerSettings(this IServiceCollection serviceCollection)
    {
        serviceCollection.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
    }
}
