﻿using ArpellaStores.Models;

namespace ArpellaStores.Services;

public interface ICouponService
{
    Task<IResult> GetCoupons();
    Task<IResult> GetCoupon(int couponId);
    Task<IResult> CreateCoupon(Coupon coupon);
    Task<IResult> UpdateCoupon(Coupon update, int id);
    Task<IResult> RemoveCoupon(int id);
}
