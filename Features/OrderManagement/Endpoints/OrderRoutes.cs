using ArpellaStores.Extensions;
using ArpellaStores.Features.OrderManagement.Models;
using ArpellaStores.Features.OrderManagement.Services;

namespace ArpellaStores.Features.OrderManagement.Endpoints;

public class OrderRoutes : IRouteRegistrar
{
    private readonly IOrderService _orderService;
    public OrderRoutes(IOrderService orderService)
    {
        _orderService = orderService;
    }
    public void RegisterRoutes(WebApplication app)
    {
        MapOrderRoutes(app);
    }
    public void MapOrderRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Orders");
        app.MapGet("/orders", () => this._orderService.GetOrders()).Produces(200).Produces(404).Produces<List<Order>>();
        app.MapGet("/order/{id}", (string id) => this._orderService.GetOrder(id)).Produces(200).Produces(404).Produces<Order>();
        app.MapPost("/order", (Order order) => this._orderService.CreateOrder(order)).Produces(200).Produces(404).Produces<Order>();
        app.MapDelete("/order/{id}", (string id) => this._orderService.RemoveOrder(id)).Produces(200).Produces(404).Produces<Order>();
    }
}
