using ArpellaStores.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Data;

public static class ServiceRegistration
{
    public static void RegisterDataServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("arpellaDB");
        serviceCollection.AddDbContext<ArpellaContext>(options =>
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }, ServiceLifetime.Singleton);
    }
}
