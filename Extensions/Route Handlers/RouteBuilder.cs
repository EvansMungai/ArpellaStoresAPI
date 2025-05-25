using ArpellaStores.Features.Authentication.Endpoints;
using ArpellaStores.Features.Delivery_Tracking_Management.Endpoints;
using ArpellaStores.Features.Final_Price_Management.Endpoints;
using ArpellaStores.Features.Goods_Information_Management.Endpoints;

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
