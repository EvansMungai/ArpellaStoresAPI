namespace ArpellaStores.Features.InventoryManagement.Services;

public interface IProductImageService
{
    Task<IResult> GetProductImageDetails();
    Task<IResult> GetProductImageUrl(int productId);
    Task<IResult> CreateProductImagesDetails(HttpRequest request);
    Task<IResult> DeleteProductImagesDetails(int id);
    Task<string> GetProductImageUrl(IFormFile formFile);
}
