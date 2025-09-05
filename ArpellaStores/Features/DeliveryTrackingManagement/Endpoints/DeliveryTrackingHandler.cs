using ArpellaStores.Extensions.RouteHandlers;
using ArpellaStores.Features.DeliveryTrackingManagement.Models;
using ArpellaStores.Features.DeliveryTrackingManagement.Services;

namespace ArpellaStores.Features.DeliveryTrackingManagement.Endpoints;

public class DeliveryTrackingHandler : IHandler
{
    public static string RouteName => "Delivery Tracking Management";
    private readonly IDeliveryTrackingService _deliveryTrackingService;
    public DeliveryTrackingHandler(IDeliveryTrackingService deliveryTrackingService)
    {
        _deliveryTrackingService = deliveryTrackingService;
    }

    public Task<IResult> CreateDelivery(Deliverytracking delivery) => _deliveryTrackingService.CreateDelivery(delivery);
    public Task<IResult> GetDeliveryStatus(string orderId) => _deliveryTrackingService.GetDeliveryStatus(orderId);
    public Task<IResult> GetAgentOrders(string deliveryAgent) => _deliveryTrackingService.GetAgentOrders(deliveryAgent);
    public Task<IResult> UpdateDeliveryStatus(string status, string orderId) => _deliveryTrackingService.UpdateDeliveryStatus(status, orderId);
}
