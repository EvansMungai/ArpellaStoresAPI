using ArpellaStores.Extensions;
using ArpellaStores.Features.FinalPriceManagement.Models;

namespace ArpellaStores.Features.FinalPriceManagement.Endpoints;

public class FinalPriceRoutes : IRouteRegistrar
{
    public void RegisterRoutes(WebApplication app)
    {
        MapFinalPriceRoutes(app);
        MapCouponRoutes(app);
        MapDiscountRoutes(app);
        MapFlashsaleRoutes(app);
    }

    public void MapFinalPriceRoutes(WebApplication webApplication)
    {
        webApplication.MapGet("/final-price", (FinalPriceHandler handler, int productId, string? couponCode) => handler.GetFinalPriceAsync(productId, couponCode)).WithTags("Final-Price");
    }
    public void MapCouponRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Coupon");
        app.MapGet("/coupons", (FinalPriceHandler handler) => handler.GetCoupons()).Produces(200).Produces(404).Produces<List<Coupon>>();
        app.MapGet("/coupon/{id}", (FinalPriceHandler handler, int id) => handler.GetCoupon(id)).Produces(200).Produces(404).Produces<Coupon>();
        app.MapPost("/coupon", (FinalPriceHandler handler, Coupon coupon) => handler.CreateCoupon(coupon)).Produces(200).Produces(500).Produces<Coupon>();
        app.MapPut("/coupon/{id}", (FinalPriceHandler handler, Coupon coupon, int id) => handler.UpdateCoupon(coupon, id)).Produces(200).Produces(404).Produces(500).Produces<Coupon>();
        app.MapDelete("/coupon/{id}", (FinalPriceHandler handler, int id) => handler.RemoveCoupon(id)).Produces(200).Produces(404).Produces(500).Produces<Coupon>();
    }
    public void MapDiscountRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Discount");
        app.MapGet("/discounts", (FinalPriceHandler handler, int productId, string couponCode) => handler.GetFinalPrice(productId, couponCode)).Produces(200).Produces(404).Produces<Discount>();
    }
    public void MapFlashsaleRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Flashsales");
        app.MapGet("/flashsales", (FinalPriceHandler handler) => handler.GetFlashSales()).Produces(200).Produces(404).Produces<List<Flashsale>>();
        app.MapGet("/flashsale/{id}", (FinalPriceHandler handler, int id) => handler.GetFlashSale(id)).Produces(200).Produces(404).Produces<Flashsale>();
        app.MapPost("/flashsale", (FinalPriceHandler handler, Flashsale flashsale) => handler.CreateFlashSale(flashsale)).Produces(200).Produces(500).Produces<Flashsale>();
        app.MapPut("/flashsale/{id}", (FinalPriceHandler handler, Flashsale flashsale, int id) => handler.UpdateFlashSale(flashsale, id)).Produces(200).Produces(404).Produces(500).Produces<Flashsale>();
        app.MapDelete("/flashsale/{id}", (FinalPriceHandler handler, int id) => handler.RemoveFlashSale(id)).Produces(200).Produces(404).Produces(500).Produces<Flashsale>();
    }
}
