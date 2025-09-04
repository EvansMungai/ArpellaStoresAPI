using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.OrderManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.OrderManagement.Services;

public class OrderFinalizerService : IOrderFinalizerService
{
    private readonly ArpellaContext _context;
    public OrderFinalizerService(ArpellaContext context)
    {
        _context = context;
    }
    public async Task FinalizeOrderAsync(Order order, string transactionId)
    {
        order.Status = "Pending";

        _context.Orders.Add(order);

        foreach (var item in order.Orderitems)
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.InventoryId == item.ProductId);

            if (inventory == null)
            {
                throw new InvalidOperationException($"Insufficient stock for product {item.ProductId}");
            }

            if (inventory.StockQuantity < item.Quantity)
                throw new InvalidOperationException($"Insufficient stock for product {item.ProductId}");

            inventory.StockQuantity -= item.Quantity;
            _context.Inventories.Update(inventory);
        }

        var payment = new Payment
        {
            Orderid = order.Orderid,
            TransactionId = transactionId,
            Status = "Completed"
        };

        _context.Payments.Add(payment);

        await _context.SaveChangesAsync();
    }
}
