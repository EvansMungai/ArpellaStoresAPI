namespace ArpellaStores.Features.FinalPriceManagement.Services;

public interface IDiscountService
{
    Task<IResult> GetFinalPrice(int productId, string couponCode = null);
}
