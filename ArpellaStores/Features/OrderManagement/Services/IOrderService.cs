using ArpellaStores.Features.OrderManagement.Models;

namespace ArpellaStores.Features.OrderManagement.Services;

public interface IOrderService
{
    Task<IResult> GetOrders();
    Task<IResult> GetOrder(string orderId);
    Task<IResult> GetOrderByUsername(string username);
    Task<IResult> GetPagedOrders(int pageNumber, int pageSize);
    Task<IResult> CreateOrder(Order orderDetails);
    //Task<IResult> UpdateOrderDetails(Order update, string id);
    Task<IResult> RemoveOrder(string orderId);
}
