using ArpellaStores.Models;

namespace ArpellaStores.Services
{
    public interface IProductsService
    {
        //List<Product> GetProducts();
        Task<IResult> GetProducts();
        Task<IResult> GetProduct(string productId);
        Task<IResult> CreateProduct(Product product);
        Task<IResult> UpdateProductDetails(Product product, string id);
        Task<IResult> UpdateProductPrice(string id, decimal price);
        Task<IResult> RemoveProduct(string productId);
    }
}
