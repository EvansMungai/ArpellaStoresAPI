using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.FinalPriceManagement.Models;

namespace ArpellaStores.Features.FinalPriceManagement.Services;

public class CouponService : ICouponService
{
    private readonly ArpellaContext _context;
    public CouponService(ArpellaContext context)
    {
        _context = context;
    }
    public async Task<IResult> GetCoupons()
    {
        List<Coupon> coupons = _context.Coupons.ToList();
        return coupons == null || coupons.Count == 0 ? Results.NotFound("No Coupons Found") : Results.Ok(coupons);
    }
    public async Task<IResult> GetCoupon(int couponId)
    {
        Coupon? coupon = _context.Coupons.SingleOrDefault(c => c.CouponId == couponId);
        return coupon == null ? Results.NotFound($"Coupon with Id = {couponId} was not found") : Results.Ok(coupon);

    }
    public async Task<IResult> CreateCoupon(Coupon coupon)
    {
        Coupon coupon1 = new Coupon
        {
            CouponId = coupon.CouponId,
            CouponCode = coupon.CouponCode,
            DiscountType = coupon.DiscountType,
            DiscountValue = coupon.DiscountValue,
            UsageLimit = coupon.UsageLimit,
            StartDate = coupon.StartDate,
            EndDate = coupon.EndDate,
            IsActive = coupon.IsActive
        };
        try
        {
            _context.Coupons.Add(coupon1);
            await _context.SaveChangesAsync();
            return Results.Ok(coupon1);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
    public async Task<IResult> UpdateCoupon(Coupon update, int id)
    {
        Coupon? retrievedCoupon = _context.Coupons.SingleOrDefault(c => c.CouponId.Equals(id));
        if (retrievedCoupon != null)
        {
            retrievedCoupon.CouponCode = update.CouponCode;
            retrievedCoupon.DiscountType = update.DiscountType;
            retrievedCoupon.DiscountValue = update.DiscountValue;
            retrievedCoupon.UsageLimit = update.UsageLimit;
            retrievedCoupon.StartDate = update.StartDate;
            retrievedCoupon.EndDate = update.EndDate;
            retrievedCoupon.IsActive = update.IsActive;
            try
            {
                _context.Coupons.Update(retrievedCoupon);
                await _context.SaveChangesAsync();
                return Results.Ok(retrievedCoupon);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }
        else
        {
            return Results.NotFound($"Coupon of Id = {id} was not found");
        }
    }
    public async Task<IResult> RemoveCoupon(int id)
    {
        Coupon? coupon = _context.Coupons.SingleOrDefault(c => c.CouponId == id);
        if (coupon != null)
        {
            try
            {
                _context.Coupons.Remove(coupon);
                await _context.SaveChangesAsync();
                return Results.Ok(coupon);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }
        else
        {
            return Results.NotFound($"Coupon of Id ={id} was not found");
        }
    }
}
