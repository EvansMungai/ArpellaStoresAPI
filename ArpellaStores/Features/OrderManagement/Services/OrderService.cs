using ArpellaStores.Features.OrderManagement.Models;

namespace ArpellaStores.Features.OrderManagement.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repo;
    private readonly IOrderPaymentService _payment;
    private readonly IOrderHelper _helper;
    private readonly IOrderCacheService _cache;

    public OrderService(IOrderRepository repo, IOrderPaymentService payment, IOrderHelper helper, IOrderCacheService cache)
    {
        _repo = repo;
        _payment = payment;
        _helper = helper;
        _cache = cache;
    }

    public async Task<IResult> CreateOrder(Order orderDetails)
    {
        if (await _repo.ExistsAsync(orderDetails.Orderid))
            return Results.Conflict($"OrderID {orderDetails.Orderid} already exists");

        var total = _repo.CalculateTotalOrderCost(orderDetails);
        var order = _helper.BuildNewOrder(orderDetails, total);

        var stk = await _payment.InitiateStkPushAsync(order);
        if (stk == null || string.IsNullOrEmpty(stk.CheckoutRequestID))
            return Results.BadRequest("STK Push failed");

        _cache.CacheOrder($"pending-order-{stk.CheckoutRequestID}", order, TimeSpan.FromMinutes(4));

        var responseData = new
        {
            Message = "STK push sent. Awaiting payment.",
            CheckoutRequestID = stk.CheckoutRequestID,
            Amount = order.Total,
            Phonenumber = order.PhoneNumber
        };

        return Results.Accepted($"/confirm-payment/{stk.CheckoutRequestID}", responseData);
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
