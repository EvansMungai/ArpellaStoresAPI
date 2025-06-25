using ArpellaStores.Extensions;
using ArpellaStores.Features.FinalPriceManagement.Models;
using ArpellaStores.Features.FinalPriceManagement.Services;

namespace ArpellaStores.Features.FinalPriceManagement.Endpoints;

public class FinalPriceHandler : IHandler
{
    public static string RouteName => "Final Price Management";
    private readonly IFinalPriceService _finalPriceService;
    private readonly ICouponService _couponService;
    private readonly IDiscountService _discountService;
    private readonly IFlashsaleService _flashsaleService;
    public FinalPriceHandler(IFinalPriceService finalPriceService, ICouponService couponService, IDiscountService discountService, IFlashsaleService flashsaleService)
    {
        _finalPriceService = finalPriceService;
        _couponService = couponService;
        _discountService = discountService;
        _flashsaleService = flashsaleService;
    }

    #region Final Price Handler 
    public Task<decimal> GetFinalPriceAsync(int productId, string couponCode) => _finalPriceService.GetFinalPriceAsync(productId, couponCode);
    #endregion

    #region Coupons Handler
    public Task<IResult> GetCoupons() => _couponService.GetCoupons();
    public Task<IResult> GetCoupon(int couponId) => _couponService.GetCoupon(couponId);
    public Task<IResult> CreateCoupon(Coupon coupon) => _couponService.CreateCoupon(coupon);
    public Task<IResult> UpdateCoupon(Coupon update, int id) => _couponService.UpdateCoupon(update, id);
    public Task<IResult> RemoveCoupon(int id) => _couponService.RemoveCoupon(id);
    #endregion

    #region Discount Handler
    public Task<IResult> GetFinalPrice(int productId, string couponCode = null) => _discountService.GetFinalPrice(productId, couponCode);
    #endregion

    #region Flashsale Handler
    public Task<IResult> GetFlashSales()=> _flashsaleService.GetFlashSales();
    public Task<IResult> GetFlashSale(int flashSaleId) => _flashsaleService.GetFlashSale(flashSaleId);
    public Task<IResult> CreateFlashSale(Flashsale flashsale) => _flashsaleService.CreateFlashSale(flashsale);
    public Task<IResult> UpdateFlashSale(Flashsale update, int id) => _flashsaleService.UpdateFlashSale(update, id);
    public Task<IResult> RemoveFlashSale(int id) => _flashsaleService.RemoveFlashsale(id);
    #endregion
}
