namespace ArpellaStores.Features.FinalPriceManagement.Models;

public partial class Coupon
{
    public int CouponId { get; set; }

    public string? CouponCode { get; set; }

    public string DiscountType { get; set; } = null!;

    public decimal DiscountValue { get; set; }

    public int? UsageLimit { get; set; }

    public int? UsageCount { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool? IsActive { get; set; }
}
