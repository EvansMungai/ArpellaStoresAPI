using ArpellaStores.Features.OrderManagement.Models;

namespace ArpellaStores.Features.OrderManagement.Services;

public class OrderHelper : IOrderHelper
{
    private readonly ILogger<OrderHelper> _logger;
    public OrderHelper(ILogger<OrderHelper> logger) { _logger = logger; }
    public string GenerateOrderId()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    //public Order BuildNewOrder(Order orderDetails, decimal totalCost)
    //{
    //    return new Order
    //    {
    //        Orderid = GenerateOrderId(),
    //        UserId = orderDetails.UserId,
    //        PhoneNumber = orderDetails.PhoneNumber,
    //        Status = "Pending",
    //        Total = totalCost,
    //        Latitude = orderDetails.Latitude,
    //        Longitude = orderDetails.Longitude,
    //        BuyerPin = orderDetails.BuyerPin,
    //        Orderitems = orderDetails.Orderitems
    //    };
    //}
    public CachedOrderDto BuildNewOrder(Order orderDetails, decimal totalCost)
    {
        return new CachedOrderDto
        {
            Orderid = GenerateOrderId(),
            UserId = orderDetails.UserId,
            PhoneNumber = orderDetails.PhoneNumber,
            Status = "Pending",
            Total = totalCost,
            Latitude = orderDetails.Latitude,
            Longitude = orderDetails.Longitude,
            BuyerPin = orderDetails.BuyerPin,
            Orderitems = orderDetails.Orderitems.Select(item => new CachedOrderItemDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList()
        };


    }
    public Order RebuildOrder(CachedOrderDto cachedOrder)
    {
        _logger.LogInformation("Rebuilding Order from Cache............");

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
            BuyerPin = cachedOrder.BuyerPin,
            Orderitems = rebuiltItems
        };
    }
}
