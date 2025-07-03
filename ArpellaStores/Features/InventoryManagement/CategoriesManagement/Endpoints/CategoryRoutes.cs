using ArpellaStores.Extensions;
using ArpellaStores.Extensions.RouteHandlers;
using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Endpoints;

public class CategoryRoutes : IRouteRegistrar
{
    public void RegisterRoutes(WebApplication app)
    {
        MapCategoryRoutes(app);
        MapSubcategoryRoutes(app);
    }

    public void MapCategoryRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Categories");
        app.MapGet("/categories", (CategoryHandler handler) => handler.GetCategories()).Produces(200).Produces(404).Produces<List<Category>>();
        app.MapGet("/category/{id}", (CategoryHandler handler ,int id) => handler.GetCategory(id)).Produces(200).Produces(404).Produces<Category>();
        app.MapPost("/category", (CategoryHandler handler, Category category) => handler.CreateCategory(category)).Produces<Category>().AddEndpointFilter<ValidationEndpointFilter<Category>>();
        app.MapPut("/category/{id}", (CategoryHandler handler, Category category, int id) => handler.UpdateCategoryDetails(category, id)).Produces<Category>().AddEndpointFilter<ValidationEndpointFilter<Category>>();
        app.MapDelete("/category/{id}", (CategoryHandler handler, int id) => handler.RemoveCategory(id)).Produces(200).Produces(404).Produces<Category>();
    }
    public void MapSubcategoryRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Subcategories");
        app.MapGet("/subcategories", (CategoryHandler handler) => handler.GetSubcategories()).Produces(200).Produces(404).Produces<List<Subcategory>>();
        app.MapGet("/subcategory/{id}", (CategoryHandler handler, int id) => handler.GetSubcategory(id)).Produces(200).Produces(404).Produces<Subcategory>();
        app.MapPost("/subcategory", (CategoryHandler handler, Subcategory subcategory) => handler.CreateSubcategory(subcategory)).Produces(200).Produces(404).Produces<Subcategory>().AddEndpointFilter<ValidationEndpointFilter<Subcategory>>();
        app.MapPut("/subcategory/{id}", (CategoryHandler handler, Subcategory subcategory, int id) => handler.UpdateSubcategoryDetails(subcategory, id)).Produces<Subcategory>().AddEndpointFilter<ValidationEndpointFilter<Subcategory>>();
        app.MapDelete("/subcategory/{id}", (CategoryHandler handler, int id) => handler.RemoveSubcategory(id)).Produces(200).Produces(404).Produces<Subcategory>();
    }
}
