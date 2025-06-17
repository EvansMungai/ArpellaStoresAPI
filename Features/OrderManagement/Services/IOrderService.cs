using ArpellaStores.Features.OrderManagement.Models;

namespace ArpellaStores.Features.OrderManagement.Services;

public interface IOrderService
{
    Task<IResult> GetOrders();
    Task<IResult> GetOrder(string id);
    Task<IResult> CreateOrder(Order category);
    //Task<IResult> UpdateOrderDetails(Order update, string id);
    Task<IResult> RemoveOrder(string categoryId);
}
