using ArpellaStores.Extensions;
using ArpellaStores.Features.Payment_Management.Models;
using ArpellaStores.Services;

namespace ArpellaStores.Features.Payment_Management.Endpoints;

public class PaymentRoutes : IRouteRegistrar
{
    private readonly IMpesaService _mpesaService;
    public PaymentRoutes(IMpesaService mpesaService)
    {
        _mpesaService = mpesaService;
    }
    public void RegisterRoutes(WebApplication app)
    {
        MapPaymentRoutes(app);
    }
    public void MapPaymentRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Mpesa");
        app.MapGet("/mpesa", () => this._mpesaService.GetToken());
        app.MapPost("/registerurls", () => this._mpesaService.RegisterUrls());
        app.MapPost("/pay", (MpesaExpressRequest mpesa) => this._mpesaService.SendPaymentPrompt(mpesa));
    }
}
