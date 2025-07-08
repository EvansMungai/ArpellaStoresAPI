using ArpellaStores.Features.OrderManagement.Models;

namespace ArpellaStores.Features.OrderManagement.Services;

public interface IOrderCacheService
{
    void CacheOrder(string key, Order order, TimeSpan expiration);
}
