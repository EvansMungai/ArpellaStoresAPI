namespace ArpellaStores.Features.FinalPriceManagement.Services;

public interface IFinalPriceService
{
    Task<decimal> GetFinalPriceAsync(int productId, string couponCode = null);
}
