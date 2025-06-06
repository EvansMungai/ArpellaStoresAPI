using ArpellaStores.Services;
using ArpellaStores.Models;
using ArpellaStores.Extensions;
using ArpellaStores.Features.Inventory_and_Product_management.Category_and_Subcategory_Managment.Services;

namespace ArpellaStores.Features.Inventory_and_Product_management.Category_and_Subcategory_Managment.Endpoints;

public class CategoryRoutes : IRouteRegistrar
{
    private readonly ICategoriesService _categoriesService;
    private readonly ISubcategoriesServices _subcategoriesService;
    public CategoryRoutes(ICategoriesService categoriesService, ISubcategoriesServices subcategoriesService)
    {
        _categoriesService = categoriesService;
        _subcategoriesService = subcategoriesService;
    }
    public void RegisterRoutes(WebApplication app)
    {
        MapCategoryRoutes(app);
        MapSubcategoryRoutes(app);
    }

    public void MapCategoryRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Categories");
        app.MapGet("/categories", () => this._categoriesService.GetCategories()).Produces(200).Produces(404).Produces<List<Category>>();
        app.MapGet("/category/{id}", (int id) => this._categoriesService.GetCategory(id)).Produces(200).Produces(404).Produces<Category>();
        app.MapPost("/category", (Category category) => this._categoriesService.CreateCategory(category)).Produces<Category>();
        app.MapPut("/category/{id}", (Category category, int id) => this._categoriesService.UpdateCategoryDetails(category, id)).Produces<Category>();
        app.MapDelete("/category/{id}", (int id) => this._categoriesService.RemoveCategory(id)).Produces(200).Produces(404).Produces<Category>();
    }
    public void MapSubcategoryRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Subcategories");
        app.MapGet("/subcategories", () => this._subcategoriesService.GetSubcategories()).Produces(200).Produces(404).Produces<List<Subcategory>>();
        app.MapGet("/subcategory/{id}", (int id) => this._subcategoriesService.GetSubcategory(id)).Produces(200).Produces(404).Produces<Subcategory>();
        app.MapPost("/subcategory", (Subcategory subcategory) => this._subcategoriesService.CreateSubcategory(subcategory)).Produces(200).Produces(404).Produces<Subcategory>();
        app.MapPut("/subcategory/{id}", (Subcategory subcategory, int id) => this._subcategoriesService.UpdateSubcategoryDetails(subcategory, id)).Produces<Subcategory>();
        app.MapDelete("/subcategory/{id}", (int id) => this._subcategoriesService.RemoveSubcategory(id)).Produces(200).Produces(404).Produces<Subcategory>();
    }
}
