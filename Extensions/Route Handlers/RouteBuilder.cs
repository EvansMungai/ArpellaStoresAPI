namespace ArpellaStores.Extensions;

public class RouteBuilder
{
    private readonly IEnumerable<IRouteRegistrar> _routeRegistrars;
    public RouteBuilder(IEnumerable<IRouteRegistrar> routeRegistrars)
    {
        _routeRegistrars = routeRegistrars;
    }
    public void RegisterRoutes(WebApplication app)
    {
        foreach(var registrar in _routeRegistrars)
        {
            registrar.RegisterRoutes(app);
        }
    }
}
