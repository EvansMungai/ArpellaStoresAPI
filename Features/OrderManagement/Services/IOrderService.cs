using ArpellaStores.Features.OrderManagement.Models;

namespace ArpellaStores.Features.OrderManagement.Services;

public interface IOrderService
{
    Task<IResult> GetOrders();
    Task<IResult> GetOrder(string orderId);
    Task<IResult> CreateOrder(Order order);
    //Task<IResult> UpdateOrderDetails(Order update, string id);
    Task<IResult> RemoveOrder(string orderId);
}
