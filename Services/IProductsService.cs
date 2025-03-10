using ArpellaStores.Models;
using Microsoft.AspNetCore.Antiforgery;

namespace ArpellaStores.Services
{
    public interface IProductsService
    {
        //List<Product> GetProducts();
        Task<IResult> GetProducts();
        Task<IResult> GetProduct(int productId);
        Task<IResult> CreateProduct(Product product);
        Task<string> GetProductImageUrl(IFormFile formFile);
        Task<IResult> GetProductImageDetails();
        Task<IResult> CreateProductImagesDetails(HttpRequest request);
        Task<IResult> DeleteProductImagesDetails(int id);
        Task<IResult> CreateProducts(IFormFile file);
        Task<IResult> UpdateProductDetails(Product product, int id);
        Task<IResult> UpdateProductPrice(int id, decimal price);
        Task<IResult> RemoveProduct(int productId);
    }
}
