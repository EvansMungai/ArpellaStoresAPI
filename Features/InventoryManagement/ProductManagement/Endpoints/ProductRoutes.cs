using ArpellaStores.Extensions;
using ArpellaStores.Features.InventoryManagement.Models;
using ArpellaStores.Features.InventoryManagement.Services;

namespace ArpellaStores.Features.InventoryManagement.Endpoints;

public class ProductRoutes : IRouteRegistrar
{
    public void RegisterRoutes(WebApplication app)
    {
        MapProductRoutes(app);
        MapProductImagesRoutes(app);
    }
    public void MapProductRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Products");
        app.MapGet("/products", (ProductHandler handler) => handler.GetProducts()).Produces(200).Produces(404).Produces<List<Product>>();
        app.MapGet("/product/{id}", (ProductHandler handler,int id) => handler.GetProduct(id)).Produces(200).Produces(404).Produces<Product>();
        app.MapPost("/product", (ProductHandler handler, Product product) => handler.CreateProduct(product)).Produces(200).Produces(404).Produces<Product>();
        app.MapPost("/products", (ProductHandler handler,IFormFile file) => handler.CreateProducts(file)).Produces(200).Produces(404).Produces<Product>().DisableAntiforgery();
        app.MapPut("/product/{id}", (ProductHandler handler, Product product, int id) => handler.UpdateProductDetails(product, id)).Produces(200).Produces(404).Produces<Product>();
        app.MapDelete("/product/{id}", (ProductHandler handler, int id) => handler.RemoveProduct(id)).Produces(200).Produces(404).Produces<Product>();
    }
    public void MapProductImagesRoutes(WebApplication webApplication) {
        var app = webApplication.MapGroup("").WithTags("Product Images");
        app.MapGet("/product-image-details", (ProductHandler handler) => handler.GetProductImageDetails());
        app.MapGet("/product-image/{id}", (ProductHandler handler, string id) => handler.GetProductImageUrl(id));
        app.MapPost("/product-image-details", (ProductHandler handler, HttpRequest request) => handler.CreateProductImageDetails(request));
        app.MapDelete("/product-image-details/{id}", (ProductHandler handler, int id) => handler.DeleteProductImageDetails(id)).Produces(200).Produces(404);
    }
}
