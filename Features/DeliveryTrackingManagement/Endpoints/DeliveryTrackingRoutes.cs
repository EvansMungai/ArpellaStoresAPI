using ArpellaStores.Extensions;
using ArpellaStores.Features.DeliveryTrackingManagement.Models;

namespace ArpellaStores.Features.DeliveryTrackingManagement.Endpoints;

public class DeliveryTrackingRoutes: IRouteRegistrar
{
    public void RegisterRoutes(WebApplication app)
    {
        MapDeliveryTrackingRoutes(app);
    }
    public void MapDeliveryTrackingRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Delivery Tracking");
        app.MapPost("/deliverytracking/{orderid}", (DeliveryTrackingHandler handler ,Deliverytracking delivery) => handler.CreateDelivery(delivery)).Produces(200).Produces(404).Produces<Deliverytracking>();
        app.MapPut("deliverytracking/{orderid}/status", (DeliveryTrackingHandler handler, string status, string orderid) => handler.UpdateDeliveryStatus(status, orderid)).Produces(200).Produces(404).Produces<Deliverytracking>();
        app.MapGet("/deliverytracking/{orderid}", (DeliveryTrackingHandler handler, string orderid) => handler.GetDeliveryStatus(orderid)).Produces(200).Produces(404).Produces<Deliverytracking>();
    }
}
