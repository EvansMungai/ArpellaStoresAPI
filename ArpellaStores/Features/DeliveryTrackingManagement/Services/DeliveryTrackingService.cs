using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.DeliveryTrackingManagement.Models;
using ArpellaStores.Features.OrderManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.DeliveryTrackingManagement.Services;

public class DeliveryTrackingService : IDeliveryTrackingService
{
    private readonly ArpellaContext _context;
    private readonly IOrderService _orderService;
    public DeliveryTrackingService(ArpellaContext context, IOrderService orderService)
    {
        _context = context;
        _orderService = orderService;
    }
    public async Task<IResult> CreateDelivery(Deliverytracking delivery)
    {
        var local = _context.Deliverytrackings.Local.FirstOrDefault(d => d.DeliveryId == delivery.DeliveryId);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        var existing = await _context.Deliverytrackings.AsNoTracking().SingleOrDefaultAsync(d => d.DeliveryId == delivery.DeliveryId);
        if (existing != null)
            return Results.Conflict($"A Delivery trackings with ID = {delivery.DeliveryId} already exists.");

        var newDelivery = new Deliverytracking
        {
            OrderId = delivery.OrderId,
            Username = delivery.Username,
            DeliveryAgent = delivery.DeliveryAgent,
            Status = "Processing"
        };
        try
        {
            await _orderService.UpdateOrderStatus(newDelivery.Status, newDelivery.OrderId);
            _context.Deliverytrackings.Add(newDelivery);
            await _context.SaveChangesAsync();
            return Results.Ok($"Order {newDelivery.OrderId} has been scheduled for delivery");
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
    }
    public async Task<IResult> GetDeliveryStatus(string orderid)
    {
        var deliveryStatus = await _context.Deliverytrackings.Where(d => d.OrderId == orderid).Select(o => new { o.DeliveryId, o.OrderId, o.Username, o.DeliveryAgent, o.Status, o.LastUpdated }).AsNoTracking().SingleOrDefaultAsync();
        return deliveryStatus == null ? Results.NotFound($"Delivery with order id = {orderid} was not found") : Results.Ok(deliveryStatus);
    }
    public async Task<IResult> GetAgentOrders(string deliveryAgent)
    {
        var deliveries = await _context.Deliverytrackings.Where(d => d.DeliveryAgent == deliveryAgent).Select(o => new { o.DeliveryId, o.OrderId, o.DeliveryAgent, o.Status }).ToListAsync();
        return deliveries == null || deliveries.Count == 0 ? Results.NotFound("No categories found") : Results.Ok(deliveries);
    }
    public async Task<IResult> UpdateDeliveryStatus(string status, string orderid)
    {
        var local = _context.Deliverytrackings.Local.FirstOrDefault(d => d.OrderId == orderid);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }
        Deliverytracking? retrievedDelivery = await _context.Deliverytrackings.SingleOrDefaultAsync(d => d.OrderId == orderid);
        if (retrievedDelivery != null)
        {
            retrievedDelivery.Status = status;
            try
            {
                await _orderService.UpdateOrderStatus(status, orderid);
                _context.Deliverytrackings.Update(retrievedDelivery);
                await _context.SaveChangesAsync();
                return Results.Ok(retrievedDelivery);
            }
            catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
        }
        else
        {
            return Results.NotFound($"Delivery with order id = {orderid} was not found");
        }
    }
}
