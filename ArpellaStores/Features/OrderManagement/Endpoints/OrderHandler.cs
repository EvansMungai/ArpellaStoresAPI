using ArpellaStores.Extensions.RouteHandlers;
using ArpellaStores.Features.OrderManagement.Models;
using ArpellaStores.Features.OrderManagement.Services;

namespace ArpellaStores.Features.OrderManagement.Endpoints;

public class OrderHandler : IHandler
{
    public static string RouteName => "Order Management";
    private readonly IOrderService _orderService;
    public OrderHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public Task<IResult> GetOrders() => _orderService.GetOrders();
    public Task<IResult> GetOrder(string orderId) => _orderService.GetOrder(orderId);
    public Task<IResult> GetPagedOrders(int pageNumber, int pageSize) => _orderService.GetPagedOrders(pageNumber, pageSize);
    public Task<IResult> CreateOrder(Order order) => _orderService.CreateOrder(order);
    public Task<IResult> RemoveOrder(string orderId) => _orderService.RemoveOrder(orderId);
}
