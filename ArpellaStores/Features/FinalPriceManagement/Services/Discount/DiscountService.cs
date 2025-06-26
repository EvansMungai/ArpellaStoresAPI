using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.InventoryManagement.Services;

namespace ArpellaStores.Features.FinalPriceManagement.Services;

public class DiscountService : IDiscountService
{
    private readonly ArpellaContext _context;
    private readonly IFinalPriceService _finalPriceService;
    private readonly IProductsService _productService;
    public DiscountService(ArpellaContext context, IFinalPriceService finalPriceService, IProductsService productsService)
    {
        _context = context;
        _finalPriceService = finalPriceService;
        _productService = productsService;
    }
    public async Task<IResult> GetFinalPrice(int productId, string couponCode = null)
    {
        try
        {
            var finalPrice = await _finalPriceService.GetFinalPriceAsync(productId, couponCode);
            await this._productService.UpdateProductPrice(productId, finalPrice);
            return Results.Ok($"Price updated for ProductId = {productId}. Check the Product in the products route to verify changes.");
        } catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
