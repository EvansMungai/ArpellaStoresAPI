using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Services;

public interface IProductsService
{
    Task<IResult> GetProducts();
    Task<IResult> GetPagedProducts(int pageNumber, int pageSize);
    Task<IResult> GetProduct(int productId);
    Task<IResult> CreateProduct(Product product);
    Task<IResult> CreateProducts(IFormFile file);
    Task<IResult> UpdateProductDetails(Product product, int id);
    Task<IResult> UpdateProductPrice(int id, decimal price);
    Task<IResult> RemoveProduct(int productId);
}
