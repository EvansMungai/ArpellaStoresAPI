using ArpellaStores.Extensions;
using ArpellaStores.Extensions.RouteHandlers;
using ArpellaStores.Features.OrderManagement.Models;

namespace ArpellaStores.Features.OrderManagement.Endpoints;

public class OrderRoutes : IRouteRegistrar
{
    public void RegisterRoutes(WebApplication app)
    {
        MapOrderRoutes(app);
    }
    public void MapOrderRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Orders");
        app.MapGet("/orders", (OrderHandler handler) => handler.GetOrders()).Produces(200).Produces(404).Produces<List<Order>>();
        app.MapGet("/order/{id}", (OrderHandler handler,string id) => handler.GetOrder(id)).Produces(200).Produces(404).Produces<Order>();
        app.MapPost("/order", (OrderHandler handler, Order order) => handler.CreateOrder(order)).Produces(202).Produces(404).Produces(400).Produces<Order>().AddEndpointFilter<ValidationEndpointFilter<Order>>();
        app.MapDelete("/order/{id}", (OrderHandler handler, string id) => handler.RemoveOrder(id)).Produces(200).Produces(404).Produces<Order>();
    }
}
