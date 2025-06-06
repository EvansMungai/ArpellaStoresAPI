namespace ArpellaStores.Features.Final_Price_Management.Services;

public interface IDiscountService
{
    Task<IResult> GetFinalPrice(int productId, string couponCode = null);
}
