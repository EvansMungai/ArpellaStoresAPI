using ArpellaStores.Features.OrderManagement.Models;

namespace ArpellaStores.Features.OrderManagement.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repo;
    private readonly IOrderPaymentService _payment;
    private readonly IOrderHelper _helper;
    private readonly IOrderCacheService _cache;
    private readonly IServiceProvider _serviceProvider;
    public OrderService(IOrderRepository repo, IOrderPaymentService payment, IOrderHelper helper, IOrderCacheService cache, IServiceProvider serviceProvider)
    {
        _repo = repo;
        _payment = payment;
        _helper = helper;
        _cache = cache;
        _serviceProvider = serviceProvider;
    }

    public async Task<IResult> CreateOrder(Order orderDetails)
    {
        if (await _repo.ExistsAsync(orderDetails.Orderid))
            return Results.Conflict($"OrderID {orderDetails.Orderid} already exists");

        var total = await _repo.CalculateTotalOrderCost(orderDetails);
        var order = _helper.BuildNewOrder(orderDetails, total);

        if (order.OrderPaymentType == "Mpesa")
        {
            var stk = await _payment.InitiateStkPushAsync(order);
            if (stk == null || string.IsNullOrEmpty(stk.CheckoutRequestID))
                return Results.BadRequest("STK Push failed");

            _cache.CacheOrder($"pending-order-{stk.CheckoutRequestID}", order, TimeSpan.FromMinutes(15));

            var responseData = new
            {
                StkPush = stk,
                Amount = order.Total,
                Phonenumber = order.PhoneNumber
            };
            return Results.Accepted($"/confirm-payment/{stk.CheckoutRequestID}", responseData);
        }
        else
        {
            order.OrderPaymentType = "Cash";
            order.OrderSource = "POS";
            var rebuiltOrder = _helper.RebuildOrder(order);
            var transactionId = "Cash";

            using var scope = _serviceProvider.CreateScope();
            var finalizer = scope.ServiceProvider.GetRequiredService<IOrderFinalizerService>();
            await finalizer.FinalizeOrderAsync(rebuiltOrder, transactionId);

            return Results.Ok("Payment processed successfully and order has been saved.");
        }
    }

    public async Task<IResult> GetOrders()
    {
        var orders = await _repo.GetAllOrdersAsync();
        if (orders == null || orders.Count == 0)
            return Results.NotFound("No Orders found");

        var formatted = orders.Select(o => new
        {
            o.Orderid,
            o.UserId,
            o.Status,
            o.Total,
            o.Latitude,
            o.Longitude,
            Orderitem = o.Orderitems.Select(
                oi => new
                {
                    oi.ProductId,
                    oi.Quantity,
                    Product = new
                    {
                        oi.Product.Name,
                        oi.Product.Price
                    }
                })
        });
        return Results.Ok(formatted);
    }

    public async Task<IResult> GetOrder(string id)
    {
        var order = await _repo.GetOrderByIdAsync(id);
        if (order == null)
            return Results.NotFound($"No order with ID = {id} was found");

        var formatted = new
        {
            order.Orderid,
            order.UserId,
            order.Status,
            order.Total,
            order.Latitude,
            order.Longitude,
            OrderItems = order.Orderitems.Select(oi => new
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
        };
        return Results.Ok(formatted);
    }
    public async Task<IResult> GetPagedOrders(int pageNumber, int pageSize)
    {
        var orders = await _repo.GetPagedOrdersAsync(pageNumber, pageSize);
        if (orders == null || orders.Count == 0)
            return Results.NotFound("No Orders found");

        var formatted = orders.Select(o => new
        {
            o.Orderid,
            o.UserId,
            o.Status,
            o.Total,
            o.Latitude,
            o.Longitude,
            Orderitem = o.Orderitems.Select(
                oi => new
                {
                    oi.ProductId,
                    oi.Quantity,
                    Product = new
                    {
                        oi.Product.Name,
                        oi.Product.Price
                    }
                })
        });
        return Results.Ok(formatted);
    }
    public async Task<IResult> RemoveOrder(string id)
    {
        var existing = await _repo.GetOrderByIdAsync(id);
        if (existing == null)
            return Results.BadRequest($"Order with ID = {id} was not found.");

        try
        {
            await _repo.RemoveOrderAsync(id);
            return Results.Ok(existing);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
    }
}
