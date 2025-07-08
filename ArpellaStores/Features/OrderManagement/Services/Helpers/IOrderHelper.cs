using ArpellaStores.Features.OrderManagement.Models;

namespace ArpellaStores.Features.OrderManagement.Services;

public interface IOrderHelper
{
    string GenerateOrderId();
    Order BuildNewOrder(Order orderDetails, decimal totalCost);
}
