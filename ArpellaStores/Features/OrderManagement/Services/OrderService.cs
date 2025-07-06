using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.InventoryManagement.Models;
using ArpellaStores.Features.OrderManagement.Models;
using ArpellaStores.Features.PaymentManagement.Models;
using ArpellaStores.Features.PaymentManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text;

namespace ArpellaStores.Features.OrderManagement.Services;

public class OrderService : IOrderService
{
    private static readonly Random random = new Random();
    private readonly IMpesaApiService _mpesaApiService;
    private readonly MpesaConfig _mpesaConfig;
    private readonly ArpellaContext _context;
    public OrderService(ArpellaContext context, IOptions<MpesaConfig> mpesaConfig, IMpesaApiService mpesaApiService)
    {
        _context = context;
        _mpesaConfig = mpesaConfig.Value;
        _mpesaApiService = mpesaApiService;
    }

    public async Task<IResult> GetOrders()
    {
        var orders = await _context.Orders.Include(o => o.Orderitems).ThenInclude(oi => oi.Product).Select(o => new
        {
            o.Orderid,
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
                    oi.Product.Price
                }
            })
        }).AsNoTracking()
    .ToListAsync();
        return orders == null || orders.Count == 0 ? Results.NotFound("NO orders found") : Results.Ok(orders);
    }

    public async Task<IResult> GetOrder(string id)
    {
        var order = await _context.Orders
    .Include(o => o.Orderitems)
        .ThenInclude(oi => oi.Product)
    .Where(o => o.Orderid == id)
    .Select(o => new
    {
        o.Orderid,
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
    }).AsNoTracking()
    .SingleOrDefaultAsync();
        return order == null ? Results.NotFound($"No order with id = {id} was found") : Results.Ok(order);
    }

    public async Task<IResult> CreateOrder(Order orderDetails)
    {
        var local = _context.Orders.Local.FirstOrDefault(o => o.Orderid == orderDetails.Orderid);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        var existing = await _context.Orders.AsNoTracking().SingleOrDefaultAsync(o => o.Orderid == orderDetails.Orderid);
        if (existing != null)
            return Results.Conflict($"An Order with OrderID = {orderDetails.Orderid} already exists.");

        var order = new Order
        {
            Orderid = GenerateOrderId(),
            UserId = orderDetails.UserId,
            PhoneNumber = orderDetails.PhoneNumber,
            Status = "Pending",
            Total = CalculateTotalCost(orderDetails),
            Latitude = orderDetails.Latitude,
            Longitude = orderDetails.Longitude,
            BuyerPin = orderDetails.BuyerPin,
        };

        try
        {
            await InitiateStkPush(order);

            _context.Orders.Add(order);

            foreach (var item in orderDetails.Orderitems)
            {
                var orderItem = new Orderitem
                {
                    OrderId = order.Orderid,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };
                var productWithInventory = await _context.Products
                        .Where(p => p.Id == orderItem.ProductId)
                        .Join(_context.Inventories,
                              product => product.InventoryId,
                              inventory => inventory.ProductId,
                              (product, inventory) => new
                              {
                                  Product = product,
                                  Inventory = inventory
                              })
                        .SingleOrDefaultAsync();
                if (productWithInventory == null)
                {
                    return Results.BadRequest($"Product with ID {orderItem.ProductId} not found in the inventory.");
                }

                if (productWithInventory.Inventory.StockQuantity < orderItem.Quantity)
                {
                    return Results.BadRequest($"Insufficient stock for product '{productWithInventory.Product.Name}'. Only {productWithInventory.Inventory.StockQuantity} left in stock.");
                }

                productWithInventory.Inventory.StockQuantity -= orderItem.Quantity;

                _context.Orderitems.Add(orderItem);
                _context.Inventories.Update(productWithInventory.Inventory);
            }

            await _context.SaveChangesAsync();
            return Results.Ok(order);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
    }

    public async Task<IResult> RemoveOrder(string id)
    {
        var local = _context.Orders.Local.FirstOrDefault(o => o.Orderid == id);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }
        
        var order = await _context.Orders.Include(o => o.Orderitems).SingleOrDefaultAsync(o => o.Orderid == id);

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
            return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
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
    private async Task<LipaNaMpesaResponseModel> InitiateStkPush(Order order)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss"); ;
        string password = Convert.ToBase64String(Encoding.UTF8.GetBytes(_mpesaConfig.BusinessShortCode + _mpesaConfig.Passkey + timestamp));

        var requestPayload = new LipaNaMpesaRequestModel
        {
            BusinessShortCode = int.Parse(_mpesaConfig.BusinessShortCode),
            Password = password,
            Timestamp = timestamp,
            TransactionType = "CustomerBuyGoodsOnline",
            Amount = order.Total,
            PartyA = order.PhoneNumber,
            PartyB = int.Parse(_mpesaConfig.BusinessShortCode),
            PhoneNumber = order.PhoneNumber,
            CallBackUrl = _mpesaConfig.CallbackUri,
            AccountReference = "ArpellaStores",
            TransactionDescription = $"Payment for Order {order.Orderid}"
        };

        string stkPushUri = "https://api.safaricom.co.ke/mpesa/stkpush/v1/processrequest";

        var response = await this._mpesaApiService.LipaNaMpesa(stkPushUri, requestPayload);

        if (response != null || !string.IsNullOrEmpty(response.MerchantRequestID))
        {
            var paymentRecord = new Payment
            {
                Orderid = order.Orderid,
                Status = "Pending",
                TransactionId = response.CheckoutRequestID
            };
            _context.Payments.Add(paymentRecord);
            await _context.SaveChangesAsync();
        }
        return response;
    }
    #endregion
}
