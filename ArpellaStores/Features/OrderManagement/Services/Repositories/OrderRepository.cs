using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.OrderManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.OrderManagement.Services;

public class OrderRepository : IOrderRepository
{
    private readonly ArpellaContext _context;
    public OrderRepository(ArpellaContext context)
    {
        _context = context;
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders.Include(o => o.Orderitems).ThenInclude(oi => oi.Product).AsNoTracking().ToListAsync();
    }
    public async Task<Order?> GetOrderByIdAsync(string id)
    {
        return await _context.Orders.Include(o => o.Orderitems).ThenInclude(oi => oi.Product).AsNoTracking().SingleOrDefaultAsync(o => o.Orderid == id);
    }
    public async Task<List<Order>> GetPagedOrdersAsync(int pageNumber, int pageSize)
    {
        return await _context.Orders.Include(o => o.Orderitems).ThenInclude(oi => oi.Product)
            .Skip((pageNumber - 1) * pageSize).Take(pageSize)
            .AsNoTracking().ToListAsync();
    }
    public async Task<bool> ExistsAsync(string orderId)
    {
        return await _context.Orders.AsNoTracking().AnyAsync(o => o.Orderid == orderId);
    }
    public async Task AddOrderAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }
    public async Task RemoveOrderAsync(string id)
    {
        var order = await _context.Orders.Include(o => o.Orderitems).SingleOrDefaultAsync(o => o.Orderid == id);

        if (order == null) return;

        _context.Orderitems.RemoveRange(order.Orderitems);
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
    }
    public async Task<decimal> CalculateTotalOrderCost(Order order)
    {
        decimal totalCost = 0;

        // Retrieve and parse delivery fee
        var deliverySetting = await _context.Settings
            .SingleOrDefaultAsync(s => s.SettingName == "Delivery Fee");

        decimal deliveryfee = 0;
        if (deliverySetting != null && decimal.TryParse(deliverySetting.SettingValue, out var fee))
        {
            deliveryfee = fee;
        }

        foreach (var item in order.Orderitems)
        {
            var product = _context.Products.SingleOrDefault(p => p.Id == item.ProductId);
            if (product is not null)
            {
                if (item.Quantity > product.DiscountQuantity)
                {
                    decimal price = (decimal)product.PriceAfterDiscount;
                    totalCost += (decimal)item.Quantity * price;
                }
                else
                {
                    decimal price = product.Price;
                    totalCost += (decimal)item.Quantity * price;
                }
                totalCost += (decimal)deliveryfee;
            }
        }
        return totalCost;
    }
}
