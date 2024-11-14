using ArpellaStores.Data;

namespace ArpellaStores.Services;

public class DiscountService : IDiscountService
{
    private readonly ArpellaContext _context;
    private readonly IFinalPriceService _finalPriceService;
    public DiscountService(ArpellaContext context, IFinalPriceService finalPriceService)
    {
        _context = context;
        _finalPriceService = finalPriceService;
    }
    public async Task<IResult> GetFinalPrice(string productId, string couponCode = null)
    {
        try
        {
            var FinalPrice = await _finalPriceService.GetFinalPriceAsync(productId, couponCode);
            return Results.Ok(new {ProductId = productId, FinalPrice = FinalPrice});
        } catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
