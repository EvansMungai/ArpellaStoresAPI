using ArpellaStores.Features.OrderManagement.Models;

namespace ArpellaStores.Features.OrderManagement.Services;

public interface IOrderFinalizerService
{
    Task FinalizeOrderAsync(Order order, string transactionId);
}
