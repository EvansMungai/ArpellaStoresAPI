using ArpellaStores.Extensions;
using ArpellaStores.Features.InventoryManagement.Models;
using ArpellaStores.Features.InventoryManagement.Services;

namespace ArpellaStores.Features.InventoryManagement.Endpoints;

public class ProductRoutes : IRouteRegistrar
{
    private readonly IProductsService _productsService;
    public ProductRoutes(IProductsService productsService)
    {
        _productsService = productsService;
    }
    public void RegisterRoutes(WebApplication app)
    {
        MapProductRoutes(app);
        MapProductImagesRoutes(app);
    }
    public void MapProductRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Products");
        app.MapGet("/products", () => this._productsService.GetProducts()).Produces(200).Produces(404).Produces<List<Product>>();
        app.MapGet("/product/{id}", (int id) => this._productsService.GetProduct(id)).Produces(200).Produces(404).Produces<Product>();
        app.MapPost("/product", (Product product) => this._productsService.CreateProduct(product)).Produces(200).Produces(404).Produces<Product>();
        app.MapPost("/products", (IFormFile file) => this._productsService.CreateProducts(file)).Produces(200).Produces(404).Produces<Product>().DisableAntiforgery();
        app.MapPut("/product/{id}", (Product product, int id) => this._productsService.UpdateProductDetails(product, id)).Produces(200).Produces(404).Produces<Product>();
        app.MapDelete("/product/{id}", (int id) => this._productsService.RemoveProduct(id)).Produces(200).Produces(404).Produces<Product>();
    }
    public void MapProductImagesRoutes(WebApplication webApplication) {
        var app = webApplication.MapGroup("").WithTags("Product Images");
        app.MapGet("/product-image-details", () => this._productsService.GetProductImageDetails());
        app.MapGet("/product-image/{id}", (string id) => this._productsService.GetProductImageUrl(id));
        app.MapPost("/product-image-details", (HttpRequest request) => this._productsService.CreateProductImagesDetails(request));
        app.MapDelete("/product-image-details/{id}", (int id) => this._productsService.DeleteProductImagesDetails(id)).Produces(200).Produces(404);
    }
}
