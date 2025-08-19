using ArpellaStores.Extensions.RouteHandlers;
using ArpellaStores.Features.PaymentManagement.Models;
using ArpellaStores.Features.PaymentManagement.Services;
using Microsoft.Extensions.Options;

namespace ArpellaStores.Features.PaymentManagement.Endpoints;

public class PaymentRoutes : IRouteRegistrar
{
    private readonly MpesaConfig _mpesaConfig;
    public PaymentRoutes(IOptions<MpesaConfig> mpesaConfig)
    {
        _mpesaConfig = mpesaConfig.Value;
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
                ShortCode = int.Parse(_mpesaConfig.BusinessShortCode),
                ResponseType = "Completed",
                ConfirmationUrl = _mpesaConfig.ConfirmationUri,
                ValidationUrl = _mpesaConfig.ValidationUri
            };
            return await handler.RegisterUrl(registerUri, requestModel);
        });
        app.MapPost("/mpesa/callback", async (HttpRequest request) =>
        {
            using var reader = new StreamReader(request.Body);
            var rawBody = await reader.ReadToEndAsync();

            await File.WriteAllTextAsync("/tmp/mpesa_callback_log.json", rawBody);
            Console.WriteLine("Raw callback received");

            return Results.Ok();
        });
        app.MapGet("/confirm-payment/{id}", async (IPaymentResultHelper helper, string id) => await helper.GetPaymentStatusAsync(id));
    }
}
