using ArpellaStores.Features.OrderManagement.Models;

namespace ArpellaStores.Features.OrderManagement.Services;

public interface IOrderRepository
{
    Task<List<Order>> GetAllOrdersAsync();
    Task<Order?> GetOrderByIdAsync(string id);
    Task<List<Order>> GetPagedOrdersAsync(int pageNumber, int pageSize);
    Task<bool> ExistsAsync(string orderId);
    Task AddOrderAsync(Order order);
    Task RemoveOrderAsync(string id);
    Task<decimal> CalculateTotalOrderCost(Order order);
}
