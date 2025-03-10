using ArpellaStores.Data;
using ArpellaStores.Models;

namespace ArpellaStores.Services;

public class FinalPriceService : IFinalPriceService
{
    private readonly ArpellaContext _context;
    public FinalPriceService(ArpellaContext context)
    {
        _context = context;
    }
    public async Task<decimal> GetFinalPriceAsync(int productId, string couponCode = null)
    {
        Product? product = _context.Products.SingleOrDefault(p => p.Id == productId);
        if (product == null) throw new Exception("Product not Found");

        decimal finalPrice = product.Price;
        // 1. Check for Flash sale
        Flashsale? flashsale = _context.Flashsales.SingleOrDefault(fs => fs.ProductId == productId && fs.IsActive == true && DateTime.UtcNow >= fs.StartTime && DateTime.UtcNow <= fs.EndTime);
        if (flashsale != null)
        {
            finalPrice -= flashsale.DiscountValue;
        }

        // 2.Check for coupon code
        if (!string.IsNullOrEmpty(couponCode))
        {
            Coupon? coupon = _context.Coupons.SingleOrDefault(c => c.CouponCode == couponCode && c.IsActive == true && DateTime.UtcNow >= c.StartDate && DateTime.UtcNow <= c.EndDate && c.UsageCount < c.UsageLimit);
            if (coupon != null)
            {
                finalPrice = ApplyDiscount(finalPrice, coupon);
                coupon.UsageCount++;
                await _context.SaveChangesAsync();
            }
        }
        return finalPrice;
    }
    #region Utilities
    private decimal ApplyDiscount(decimal price, Coupon coupon)
    {
        return coupon.DiscountType.ToLower() switch
        {
            "percentage" => price - (price * coupon.DiscountValue / 100),
            "fixed" => price - coupon.DiscountValue,
            _ => price
        };
    }
    #endregion
}
