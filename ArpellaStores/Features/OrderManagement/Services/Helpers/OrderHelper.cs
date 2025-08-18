using ArpellaStores.Features.OrderManagement.Models;

namespace ArpellaStores.Features.OrderManagement.Services;

public class OrderHelper : IOrderHelper
{
    public string GenerateOrderId()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public Order BuildNewOrder(Order orderDetails, decimal totalCost)
    {
        return new Order
        {
            Orderid = GenerateOrderId(),
            UserId = orderDetails.UserId,
            PhoneNumber = orderDetails.PhoneNumber,
            Status = "Pending",
            Total = totalCost,
            Latitude = orderDetails.Latitude,
            Longitude = orderDetails.Longitude,
            BuyerPin = orderDetails.BuyerPin,
            Orderitems = orderDetails.Orderitems
        };
    }
    public Order RebuildOrder(Order cachedOrder)
    {
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
            Orderitems = cachedOrder.Orderitems.Select(item => new Orderitem
            {
                OrderId = cachedOrder.Orderid,
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList()
        };
    }

}
