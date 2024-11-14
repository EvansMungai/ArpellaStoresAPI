namespace ArpellaStores.Services;

public interface IFinalPriceService
{
    Task<decimal> GetFinalPriceAsync(string productId, string couponCode = null);
}
