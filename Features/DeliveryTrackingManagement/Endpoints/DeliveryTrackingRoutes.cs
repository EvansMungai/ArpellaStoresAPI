using ArpellaStores.Extensions;
using ArpellaStores.Features.DeliveryTrackingManagement.Models;
using ArpellaStores.Features.DeliveryTrackingManagement.Services;

namespace ArpellaStores.Features.DeliveryTrackingManagement.Endpoints;

public class DeliveryTrackingRoutes: IRouteRegistrar
{
    private readonly IDeliveryTrackingService _deliveryTrackingService;
    public DeliveryTrackingRoutes(IDeliveryTrackingService deliveryTrackingService)
    {
        _deliveryTrackingService = deliveryTrackingService;
    }
    public void RegisterRoutes(WebApplication app)
    {
        MapDeliveryTrackingRoutes(app);
    }
    public void MapDeliveryTrackingRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Delivery Tracking");
        app.MapPost("/deliverytracking/{orderid}", (Deliverytracking delivery) => this._deliveryTrackingService.CreateDelivery(delivery)).Produces(200).Produces(404).Produces<Deliverytracking>();
        app.MapPut("deliverytracking/{orderid}/status", (string status, string orderid) => this._deliveryTrackingService.UpdateDeliveryStatus(status, orderid)).Produces(200).Produces(404).Produces<Deliverytracking>();
        app.MapGet("/deliverytracking/{orderid}", (string orderid)=> this._deliveryTrackingService.GetDeliveryStatus(orderid)).Produces(200).Produces(404).Produces<Deliverytracking>();
    }
}
