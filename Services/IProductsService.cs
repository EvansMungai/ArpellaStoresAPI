using ArpellaStores.Models;
using Microsoft.AspNetCore.Antiforgery;

namespace ArpellaStores.Services
{
    public interface IProductsService
    {
        //List<Product> GetProducts();
        Task<IResult> GetProducts();
        Task<IResult> GetProduct(string productId);
        Task<IResult> CreateProduct(Product product);
        //Task<IResult> CreateProducts(IFormFile file, IAntiforgery antiforgery, HttpContext context);
        Task<IResult> UpdateProductDetails(Product product, string id);
        Task<IResult> UpdateProductPrice(string id, decimal price);
        Task<IResult> RemoveProduct(string productId);
    }
}
