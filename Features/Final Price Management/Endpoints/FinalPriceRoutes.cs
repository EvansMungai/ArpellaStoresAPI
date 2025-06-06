using ArpellaStores.Extensions;
using ArpellaStores.Features.Final_Price_Management.Models;
using ArpellaStores.Features.Final_Price_Management.Services;

namespace ArpellaStores.Features.Final_Price_Management.Endpoints;

public class FinalPriceRoutes : IRouteRegistrar
{
    private readonly IFinalPriceService _finalPriceService;
    private readonly ICouponService _couponService;
    private readonly IFlashsaleService _flashsaleService;
    public FinalPriceRoutes(IFinalPriceService finalPriceService, ICouponService couponService, IFlashsaleService flashsaleService)
    {
        _finalPriceService = finalPriceService;
        _couponService = couponService;
        _flashsaleService = flashsaleService;   
    }

    public void RegisterRoutes(WebApplication app)
    {
        MapFinalPriceRoutes(app);
        MapCouponRoutes(app);
        MapFlashsaleRoutes(app);
    }

    public void MapFinalPriceRoutes(WebApplication webApplication)
    {
        webApplication.MapGet("/final-price", (int productId, string? couponCode) => _finalPriceService.GetFinalPriceAsync(productId, couponCode));
    }
    public void MapCouponRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Coupon");
        app.MapGet("/coupons", () => this._couponService.GetCoupons()).Produces(200).Produces(404).Produces<List<Coupon>>();
        app.MapGet("/coupon/{id}", (int id) => this._couponService.GetCoupon(id)).Produces(200).Produces(404).Produces<Coupon>();
        app.MapPost("/coupon", (Coupon coupon) => this._couponService.CreateCoupon(coupon)).Produces(200).Produces(500).Produces<Coupon>();
        app.MapPut("/coupon/{id}", (Coupon coupon, int id) => this._couponService.UpdateCoupon(coupon, id)).Produces(200).Produces(404).Produces(500).Produces<Coupon>();
        app.MapDelete("/coupon/{id}", (int id) => this._couponService.RemoveCoupon(id)).Produces(200).Produces(404).Produces(500).Produces<Coupon>();
    }
    public void MapFlashsaleRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Flashsales");
        app.MapGet("/flashsales", () => this._flashsaleService.GetFlashSales()).Produces(200).Produces(404).Produces<List<Flashsale>>();
        app.MapGet("/flashsale/{id}", (int id) => this._flashsaleService.GetFlashSale(id)).Produces(200).Produces(404).Produces<Flashsale>();
        app.MapPost("/flashsale", (Flashsale flashsale) => this._flashsaleService.CreateFlashSale(flashsale)).Produces(200).Produces(500).Produces<Flashsale>();
        app.MapPut("/flashsale/{id}", (Flashsale flashsale, int id) => this._flashsaleService.UpdateFlashSale(flashsale, id)).Produces(200).Produces(404).Produces(500).Produces<Flashsale>();
        app.MapDelete("/flashsale/{id}", (int id) => this._flashsaleService.RemoveFlashsale(id)).Produces(200).Produces(404).Produces(500).Produces<Flashsale>();
    }
}
