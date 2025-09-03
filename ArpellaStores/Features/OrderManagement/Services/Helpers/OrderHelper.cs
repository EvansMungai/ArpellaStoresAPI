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
}
