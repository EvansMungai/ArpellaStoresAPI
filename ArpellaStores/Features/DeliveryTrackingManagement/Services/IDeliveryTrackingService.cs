using ArpellaStores.Features.DeliveryTrackingManagement.Models;

namespace ArpellaStores.Features.DeliveryTrackingManagement.Services;

public interface IDeliveryTrackingService
{
    Task<IResult> CreateDelivery(Deliverytracking delivery);
    Task<IResult> GetDeliveryStatus(string orderid);
    Task<IResult> GetAgentOrders(string deliveryAgent);
    Task<IResult> UpdateDeliveryStatus(string status, string orderid);
}
