using ArpellaStores.Features.OrderManagement.Models;

namespace ArpellaStores.Features.OrderManagement.Services;

public class OrderHelper : IOrderHelper
{
    private readonly IOrderPaymentService _payment;
    private readonly IOrderCacheService _cache;
    private readonly IOrderRepository _repo;
    private readonly IServiceProvider _serviceProvider;
    public OrderHelper(IOrderPaymentService payment, IOrderCacheService cache, IOrderRepository repo, IServiceProvider serviceProvider)
    {
        _payment = payment;
        _cache = cache;
        _repo = repo;
        _serviceProvider = serviceProvider;
    }
    public string GenerateOrderId()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    public CachedOrderDto BuildNewOrder(Order orderDetails, decimal totalCost)
    {

        var dtoItems = orderDetails.Orderitems
            .Select(item => new CachedOrderItemDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            })
            .ToList();

        var dto = new CachedOrderDto
        {
            Orderid = GenerateOrderId(),
            UserId = orderDetails.UserId,
            PhoneNumber = orderDetails.PhoneNumber,
            Status = "Pending",
            Total = totalCost,
            Latitude = orderDetails.Latitude,
            Longitude = orderDetails.Longitude,
            OrderPaymentType = orderDetails.OrderPaymentType,
            BuyerPin = orderDetails.BuyerPin,
            Orderitems = dtoItems,
            OrderSource = orderDetails.OrderSource
        };

        return dto;
    }
    public Order RebuildOrder(CachedOrderDto cachedOrder)
    {

        var rebuiltItems = cachedOrder.Orderitems.Select(item => new Orderitem
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity
        }).ToList();

        return new Order
        {
            Orderid = cachedOrder.Orderid,
            UserId = cachedOrder.UserId,
            PhoneNumber = cachedOrder.PhoneNumber,
            Status = "Completed",
            Total = cachedOrder.Total,
            Latitude = cachedOrder.Latitude,
            Longitude = cachedOrder.Longitude,
            OrderPaymentType = cachedOrder.OrderPaymentType,
            BuyerPin = cachedOrder.BuyerPin,
            Orderitems = rebuiltItems,
            OrderSource = cachedOrder.OrderSource
        };
    }
    public async Task<IResult> HandleMpesaOrders(CachedOrderDto order)
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
        order.OrderSource = "Ecommerce";
        return Results.Accepted($"/confirm-payment/{stk.CheckoutRequestID}", responseData);
    }
    public async Task<IResult> HandleCashOrders(CachedOrderDto order)
    {
        order.OrderPaymentType = "Cash";
        order.OrderSource = "POS";
        var rebuiltOrder = RebuildOrder(order);
        var transactionId = "Cash";

        using var scope = _serviceProvider.CreateScope();
        var finalizer = scope.ServiceProvider.GetRequiredService<IOrderFinalizerService>();
        await finalizer.FinalizeOrderAsync(rebuiltOrder, transactionId);

        return Results.Ok("Payment processed successfully and order has been saved.");
    }
    public async Task<IResult> HandleHybridOrders(CachedOrderDto order, decimal total)
    {
        order.OrderPaymentType = "Hybrid";
        order.OrderSource = "Hybrid";
       

        var stk = await _payment.InitiateStkPushAsync(order);
        if (stk == null || string.IsNullOrEmpty(stk.CheckoutRequestID))
            return Results.BadRequest("STK Push failed");
        order.Total = total;

        _cache.CacheOrder($"pending-order-{stk.CheckoutRequestID}", order, TimeSpan.FromMinutes(15));

        var responseData = new
        {
            StkPush = stk,
            Amount = order.Total,
            Phonenumber = order.PhoneNumber
        };
        return Results.Accepted($"/confirm-payment/{stk.CheckoutRequestID}", responseData);
    }
}
