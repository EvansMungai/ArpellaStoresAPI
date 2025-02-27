using ArpellaStores.Data;
using ArpellaStores.Models;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Services;

public class OrderService : IOrderService
{
    private static Random random = new Random();
    private readonly ArpellaContext _context;
    public OrderService(ArpellaContext context)
    {
        _context = context;
    }

    public async Task<IResult> GetOrders()
    {
        var orders = _context.Orders.Include(o => o.Orderitems).ThenInclude(oi => oi.Product).Select(o => new
        {
            o.OrderId,
            o.UserId,
            o.Status,
            o.Total,
            OrderItems = o.Orderitems.Select(oi => new
            {
                oi.OrderId,
                oi.ProductId,
                oi.Quantity,
                Product = new
                {
                    oi.Product.Id, // Add other product details as needed
                    oi.Product.Name,
                    oi.Product.Price
                }
            })
        })
    .ToList();
        return orders == null || orders.Count == 0 ? Results.NotFound("NO orders found") : Results.Ok(orders);
    }

    public async Task<IResult> GetOrder(string id)
    {
        var order = _context.Orders
    .Include(o => o.Orderitems)
        .ThenInclude(oi => oi.Product)
    .Where(o => o.OrderId == id)
    .Select(o => new
    {
        o.OrderId,
        o.UserId,
        o.Status,
        o.Total,
        OrderItems = o.Orderitems.Select(oi => new
        {
            oi.OrderId,
            oi.ProductId,
            oi.Quantity,
            Product = new
            {
                oi.Product.Id,
                oi.Product.Name,
            }
        })
    })
    .SingleOrDefault();
        return order == null ? Results.NotFound($"No order with id = {id} was found") : Results.Ok(order);
    }

    public async Task<IResult> CreateOrder(Order orderDetails)
    {
        var order = new Order
        {
            OrderId = GenerateOrderId(),
            UserId = orderDetails.UserId,
            Status = orderDetails.Status,
            Total = CalculateTotalCost(orderDetails),
        };

        try
        {
            _context.Orders.Add(order);

            foreach (var item in orderDetails.Orderitems)
            {
                var orderItem = new Orderitem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };
                _context.Orderitems.Add(orderItem);
            }

            await _context.SaveChangesAsync();
            return Results.Ok(order);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.InnerException.Message);
        }
    }

    public async Task<IResult> RemoveOrder(string id)
    {

        var order = await _context.Orders.Include(o => o.Orderitems).SingleOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
        {
            return Results.BadRequest($"Order with id = {id} was not found");
        }
        try
        {
            _context.Orderitems.RemoveRange(order.Orderitems);

            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();
            return Results.Ok(order);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.InnerException.Message);
        }
    }


    #region Utilities
    public static string GenerateOrderId()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
    }
    public decimal CalculateTotalCost(Order order)
    {
        decimal totalCost = 0;
        foreach (var item in order.Orderitems)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == item.ProductId);
            if (product != null)
            {
                decimal price = product.PriceAfterDiscount ?? product.Price;
                totalCost += (decimal)(item.Quantity * price);
            }
        }
        return totalCost;
    }
    #endregion
}
