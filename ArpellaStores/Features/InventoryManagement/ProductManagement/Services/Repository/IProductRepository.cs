using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Services;

public interface IProductRepository
{
    Task<List<Product>> GetAllProductsAsync();
    Task<List<Product>> GetPagedProductsAsync(int pageNumber, int pageSize);
    Task<Product?> GetProductByIdAsync(int id);
    Task AddProductAsync(Product product);
    Task AddProductsAsync(List<Product> products);
    Task UpdateProductDetails(Product product, int id);
    Task UpdateProductPrice(int id, decimal price);
    Task RemoveProduct(int id);
}
