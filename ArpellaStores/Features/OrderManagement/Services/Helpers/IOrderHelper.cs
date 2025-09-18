using ArpellaStores.Features.OrderManagement.Models;

namespace ArpellaStores.Features.OrderManagement.Services;

public interface IOrderHelper
{
    string GenerateOrderId();
    //Order BuildNewOrder(Order orderDetails, decimal totalCost);
    CachedOrderDto BuildNewOrder(Order orderDetails, decimal totalCost);
    Order RebuildOrder(CachedOrderDto cachedOrder);
    Task<IResult> HandleMpesaOrders(CachedOrderDto order);
    Task<IResult> HandleCashOrders(CachedOrderDto order);
    Task<IResult> HandleHybridOrders(CachedOrderDto order);
}
