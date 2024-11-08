using ArpellaStores.Models;

namespace ArpellaStores.Services
{
    public interface IProductManagement
    {
        List<Product> GetProducts();
        List<Product> GetProduct(string productId);
        Product CreateProduct(Product product);
        Product? UpdateProductDetails(Product product, string id);
        Product? RemoveProduct(string productId);
    }
}
