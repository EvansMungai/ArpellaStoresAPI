using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.DeliveryTrackingManagement.Models;

namespace ArpellaStores.Features.DeliveryTrackingManagement.Services;

public class DeliveryTrackingService : IDeliveryTrackingService
{
    private readonly ArpellaContext _context;
    public DeliveryTrackingService(ArpellaContext context)
    {
        _context = context;
    }
    public async Task<IResult> CreateDelivery(Deliverytracking delivery)
    {
        var newDelivery = new Deliverytracking
        {
            OrderId = delivery.OrderId,
            Username = delivery.Username,
            DeliveryAgent = delivery.DeliveryAgent,
            Status = "Pending"
        };
        try
        {
            _context.Deliverytrackings.Add(newDelivery);
            await _context.SaveChangesAsync();
            return Results.Ok($"Order {newDelivery.OrderId} has been scheduled for delivery");
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message); }
    }
    public async Task<IResult> GetDeliveryStatus(string orderid)
    {
        var deliveryStatus = _context.Deliverytrackings.Where(d => d.OrderId == orderid).Select(o => new { o.DeliveryId, o.OrderId, o.Username, o.DeliveryAgent, o.Status, o.LastUpdated }).FirstOrDefault();
        return deliveryStatus == null ? Results.NotFound($"Delivery with order id = {orderid} was not found") : Results.Ok(deliveryStatus);
    }
    public async Task<IResult> UpdateDeliveryStatus(string status, string orderid)
    {
        Deliverytracking? retrievedDelivery = _context.Deliverytrackings.FirstOrDefault(d => d.OrderId == orderid);
        if (retrievedDelivery != null)
        {
            retrievedDelivery.Status = status;
            try
            {
                _context.Deliverytrackings.Update(retrievedDelivery);
                await _context.SaveChangesAsync();
                return Results.Ok(retrievedDelivery);
            }
            catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message); }
        }
        else
        {
            return Results.NotFound($"Delivery with order id = {orderid} was not found");
        }
    }
}
