namespace ArpellaStores.Services;

public interface IDiscountService
{
    Task<IResult> GetFinalPrice(string productId, string couponCode = null);
}
