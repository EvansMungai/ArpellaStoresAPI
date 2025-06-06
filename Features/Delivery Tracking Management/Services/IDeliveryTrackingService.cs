using ArpellaStores.Features.Delivery_Tracking_Management.Models;

namespace ArpellaStores.Features.Delivery_Tracking_Management.Services;

public interface IDeliveryTrackingService
{
    Task<IResult> CreateDelivery(Deliverytracking delivery);
    Task<IResult> GetDeliveryStatus(string orderid);
    Task<IResult> UpdateDeliveryStatus(string status, string orderid);
}
