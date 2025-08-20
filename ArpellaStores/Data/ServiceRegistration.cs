using ArpellaStores.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Data;

public static class ServiceRegistration
{
    public static void RegisterDataServices(this IServiceCollection serviceCollection)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__arpellaDB");
        serviceCollection.AddDbContext<ArpellaContext>(options =>
        {
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 35)));
        }, ServiceLifetime.Scoped);
    }
}