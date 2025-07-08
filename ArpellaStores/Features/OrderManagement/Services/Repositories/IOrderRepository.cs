using ArpellaStores.Features.OrderManagement.Models;

namespace ArpellaStores.Features.OrderManagement.Services;

public interface IOrderRepository
{
    Task<List<Order>> GetAllOrdersAsync();
    Task<Order?> GetOrderByIdAsync(string id);
    Task<bool> ExistsAsync(string orderId);
    Task AddOrderAsync(Order order);
    Task RemoveOrderAsync(string id);
    decimal CalculateTotalOrderCost(Order order);
    Task FinalizeOrderAsync(Order order, string transactionId);
}
