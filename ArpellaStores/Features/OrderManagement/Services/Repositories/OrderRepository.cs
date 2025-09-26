using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.OrderManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.OrderManagement.Services;

public class OrderRepository : IOrderRepository
{
    private readonly ArpellaContext _context;
    private readonly ILogger<OrderRepository> _logger;
    public OrderRepository(ArpellaContext context, ILogger<OrderRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders.Include(o => o.Orderitems).ThenInclude(oi => oi.Product).AsNoTracking().ToListAsync();
    }
    public async Task<Order?> GetOrderByIdAsync(string id)
    {
        return await _context.Orders.Include(o => o.Orderitems).ThenInclude(oi => oi.Product).AsNoTracking().SingleOrDefaultAsync(o => o.Orderid == id);
    }
    public async Task<List<Order?>> GetOrdersByUsernameAsync(string username)
    {
        return await _context.Orders.Include(o => o.Orderitems).ThenInclude(oi => oi.Product).AsNoTracking().Where(o => o.UserId == username).ToListAsync();
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
    public async Task UpdateOrderStatusAsync(string status, string id)
    {
        var order = await _context.Orders.Include(o => o.Orderitems).SingleOrDefaultAsync(o => o.Orderid == id);
        if (order == null) return;

        order.Status = status;
        _context.Orders.Update(order);
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
        decimal deliveryfee = 0;

        if (order.OrderSource == "Ecommerce")
        {
            // Retrieve and parse delivery fee
            var deliverySetting = await _context.Settings
                .SingleOrDefaultAsync(s => s.SettingName == "Delivery Fee");

            if (deliverySetting != null && decimal.TryParse(deliverySetting.SettingValue, out var fee))
            {
                deliveryfee = fee;
            }
            totalCost = CalculateOrderPrice(order, deliveryfee);
            return totalCost;
        }
        else
        {
            totalCost = CalculateOrderPrice(order, deliveryfee);
            return totalCost;
        }
    }
    #region Helpers
    private decimal CalculateOrderPrice(Order order, decimal deliveryfee)
    {
        decimal totalPrice = 0;
        foreach (var item in order.Orderitems)
        {
            var product = _context.Products.SingleOrDefault(p => p.Id == item.ProductId);
            if (product is not null)
            {
                _logger.LogInformation("Calculating product price");
                if (item.PriceType == "Discounted")
                {
                    _logger.LogInformation("Calculating price of a discounted item.");
                    decimal price = (decimal)product.PriceAfterDiscount;
                    totalPrice += (decimal)item.Quantity * price;
                    _logger.LogInformation($"This is the total price: {totalPrice}");
                }
                else
                {
                    _logger.LogInformation("Calculating price of an item.");
                    decimal price = product.Price;
                    totalPrice += (decimal)item.Quantity * price;
                    _logger.LogInformation($"This is the total price: {totalPrice}");
                }
                totalPrice += (decimal)deliveryfee;
            }
        }
        return totalPrice;
    }
    #endregion
}
