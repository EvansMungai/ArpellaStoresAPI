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
        return await _context.Orders.Include(o => o.Orderitems).ThenInclude(oi => oi.Product).AsNoTracking().SingleOrDefaultAsync();
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
    public async Task FinalizeOrderAsync(Order order, string transactionId)
    {
        _logger.LogInformation("Beginning Finalize Order Async function");
        order.Status = "Paid";
        _context.Orders.Add(order);

        _logger.LogInformation("Beginning to read inventories to be bought");
        foreach (var item in order.Orderitems)
        {
            _logger.LogInformation($"Processing item for ProductId: {item.ProductId}, Quantity: {item.Quantity}");

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.InventoryId == item.ProductId);

            if (inventory == null)
            {
                _logger.LogWarning("Inventory not found for ProductId: {ProductId}", item.ProductId);
                throw new InvalidOperationException($"Insufficient stock for product {item.ProductId}");
            }

            if (inventory.StockQuantity < item.Quantity)
            {
                _logger.LogWarning("Insufficient stock for ProductId: {ProductId}. Available: {Available}, Required: {Required}",
                    item.ProductId, inventory.StockQuantity, item.Quantity);
                throw new InvalidOperationException($"Insufficient stock for product {item.ProductId}");
            }

            _logger.LogInformation("Updating Inventory Quantity.");
            inventory.StockQuantity -= item.Quantity;
            _context.Inventories.Update(inventory);
            _logger.LogInformation("Finished updating Inventory Quantity.");
        }

        _logger.LogInformation("Creating Payment record object to be able to store it to the db");
        var payment = new Payment
        {
            Orderid = order.Orderid,
            TransactionId = transactionId,
            Status = "Completed"
        };
        _logger.LogInformation("Add payment record to table");
        _context.Payments.Add(payment);
        _logger.LogInformation("Finished adding payment record to the db.");

        _logger.LogInformation("Save changes made to the DB.");
        await _context.SaveChangesAsync();
        _logger.LogInformation("Finish saving orders, orderitems and payment records to table");
    }
}
