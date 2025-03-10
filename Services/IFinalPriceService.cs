namespace ArpellaStores.Services;

public interface IFinalPriceService
{
    Task<decimal> GetFinalPriceAsync(int productId, string couponCode = null);
}
