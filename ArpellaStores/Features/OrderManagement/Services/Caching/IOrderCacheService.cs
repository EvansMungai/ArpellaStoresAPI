using ArpellaStores.Features.OrderManagement.Models;

namespace ArpellaStores.Features.OrderManagement.Services;

public interface IOrderCacheService
{
    void CacheOrder(string key, CachedOrderDto order, TimeSpan expiration);
}
