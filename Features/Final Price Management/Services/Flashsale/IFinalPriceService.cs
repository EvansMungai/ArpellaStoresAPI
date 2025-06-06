namespace ArpellaStores.Features.Final_Price_Management.Services;

public interface IFinalPriceService
{
    Task<decimal> GetFinalPriceAsync(int productId, string couponCode = null);
}
