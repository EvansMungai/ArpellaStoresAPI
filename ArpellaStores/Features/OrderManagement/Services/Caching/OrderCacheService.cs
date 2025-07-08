using ArpellaStores.Features.OrderManagement.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ArpellaStores.Features.OrderManagement.Services;

public class OrderCacheService : IOrderCacheService
{
    private readonly IMemoryCache _cache;
    public OrderCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void CacheOrder(string key, Order order, TimeSpan expiration)
    {
        _cache.Set(key, order, expiration);
    }
}
