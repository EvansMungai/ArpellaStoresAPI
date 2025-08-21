using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.OrderManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.OrderManagement.Services;

public class OrderFinalizerService : IOrderFinalizerService
{
    private readonly ArpellaContext _context;
    private readonly ILogger<OrderFinalizerService> _logger;
    public OrderFinalizerService(ArpellaContext context, ILogger<OrderFinalizerService> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task FinalizeOrderAsync(Order order, string transactionId)
    {
        order.Status = "Paid";

        _context.Orders.Add(order);

        _logger.LogInformation("Beginning to read inventories to be bought");
        foreach (var item in order.Orderitems)
        {
            _logger.LogInformation($"Processing item for ProductId: {item.ProductId}, Quantity: {item.Quantity}");
            _logger.LogInformation($"DB Connection Type: {_context.Database.GetDbConnection().GetType()} and the state is {_context.Database.GetDbConnection().State}");
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
