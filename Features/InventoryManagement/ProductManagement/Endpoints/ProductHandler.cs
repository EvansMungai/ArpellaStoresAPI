using ArpellaStores.Extensions;
using ArpellaStores.Features.InventoryManagement.Models;
using ArpellaStores.Features.InventoryManagement.Services;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Identity.Client;

namespace ArpellaStores.Features.InventoryManagement.Endpoints;

public class ProductHandler : IHandler
{
    public static string RouteName => "Product Managment";
    private readonly IProductsService _productsService;
    public ProductHandler(IProductsService productsService)
    {
        _productsService = productsService;
    }

    #region Products Handlers
    public Task<IResult> GetProducts() => _productsService.GetProducts();
    public Task<IResult> GetProduct(int productId) => _productsService.GetProduct(productId);
    public Task<IResult> CreateProduct(Product product) => _productsService.CreateProduct(product);
    public Task<IResult> CreateProducts(IFormFile file) => _productsService.CreateProducts(file);
    public Task<IResult> UpdateProductDetails(Product product, int id) => _productsService.UpdateProductDetails(product, id);
    public Task<IResult> UpdateProductPrice(int id, decimal price) => _productsService.UpdateProductPrice(id, price);
    public Task<IResult> RemoveProduct(int productId) =>  _productsService.RemoveProduct(productId);
    #endregion

    #region Product Images Handlers
    public Task<string> GetProductImageUrl(IFormFile file) => _productsService.GetProductImageUrl(file);
    public Task<IResult> GetProductImageDetails() => _productsService.GetProductImageDetails();
    public Task<IResult> GetProductImageUrl(string productId) => _productsService.GetProductImageUrl(productId);
    public Task<IResult> CreateProductImageDetails(HttpRequest request) => _productsService.CreateProductImagesDetails(request);
    public Task<IResult> DeleteProductImageDetails(int id) => _productsService.DeleteProductImagesDetails(id);
    #endregion
}
