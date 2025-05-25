using System.Reflection;

namespace ArpellaStores.Extensions;

public static class RouteRegistrarSetup
{
    public static void RegisterRouteRegistrars(this IServiceCollection services)
    {
        var routeRegistrarType = typeof(IRouteRegistrar);
        var registrars = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => routeRegistrarType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var registrar in registrars)
        {
            services.AddScoped(routeRegistrarType, registrar);
        }
    }
}