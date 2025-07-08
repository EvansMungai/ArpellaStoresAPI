using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Extensions.RouteHandlers;
using ArpellaStores.Features.PaymentManagement.Models;
using ArpellaStores.Features.PaymentManagement.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ArpellaStores.Features.PaymentManagement.Endpoints;

public class PaymentRoutes : IRouteRegistrar
{
    private readonly IMpesaApiService _mpesaApiService;
    private readonly MpesaConfig _mpesaConfig;
    private readonly ArpellaContext _context;
    private readonly IMemoryCache _cache;

    public PaymentRoutes(IMpesaApiService mpesaApiService, IOptions<MpesaConfig> mpesaConfig, ArpellaContext context, IMemoryCache cache)
    {
        _mpesaApiService = mpesaApiService;
        _mpesaConfig = mpesaConfig.Value;
        _context = context;
        _cache = cache;
    }

    public void RegisterRoutes(WebApplication app)
    {
        MapPaymentRoutes(app);
    }

    public void MapPaymentRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Mpesa");
        app.MapGet("/access-token", (PaymentHandler handler) => handler.GenerateAccessToken());
        app.MapPost("register-url", async (PaymentHandler handler) =>
        {
            string registerUri = "https://api.safaricom.co.ke/mpesa/c2b/v1/registerurl";
            var requestModel = new RegisterUrlRequestModel
            {
                ShortCode = 5142142,
                ResponseType = "Completed",
                ConfirmationUrl = _mpesaConfig.ConfirmationUri,
                ValidationUrl = _mpesaConfig.ValidationUri
            };
            return await handler.RegisterUrl(registerUri, requestModel);
        });
        app.MapPost("/mpesa/callback", async (MpesaCallbackModel callback, IMpesaCallbackHandler handler) => { return await handler.HandleAsync(callback); });
    }
}
