using System.Reflection;

namespace ArpellaStores.Extensions.RouteHandlers;

public static class HandlerRegistrarSetup
{
    public static void RegisterHandlers(this IServiceCollection services)
    {
        var handlerRegistrarType = typeof(IHandler);
        var registrars = Assembly.GetExecutingAssembly().GetTypes().Where(t => handlerRegistrarType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
        foreach (var registrar in registrars)
        {
            services.AddScoped(handlerRegistrarType, registrar);
            services.AddScoped(registrar);
        }
    }
}
