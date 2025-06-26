using ArpellaStores.Extensions.RouteHandlers;
using ArpellaStores.Features.InventoryManagement.Models;
using ArpellaStores.Features.InventoryManagement.Services;

namespace ArpellaStores.Features.InventoryManagement.Endpoints;

public class CategoryHandler : IHandler
{
    public static string RouteName => "Category Management";
    private readonly ICategoriesService _categoriesService;
    private readonly ISubcategoriesServices _subcategoriesServices;

    public CategoryHandler(ICategoriesService categoriesService, ISubcategoriesServices subcategoriesServices)
    {
        _categoriesService = categoriesService;
        _subcategoriesServices = subcategoriesServices;
    }

    #region Category Handlers
    public Task<IResult> GetCategories() => _categoriesService.GetCategories();
    public Task<IResult> GetCategory(int id) => _categoriesService.GetCategory(id);
    public Task<IResult> CreateCategory(Category category) => _categoriesService.CreateCategory(category);
    public Task<IResult> UpdateCategoryDetails(Category update, int id) => _categoriesService.UpdateCategoryDetails(update, id);
    public Task<IResult> RemoveCategory(int id) => _categoriesService.RemoveCategory(id);
    #endregion

    #region Subcategory handlers
    public Task<IResult> GetSubcategories() => _subcategoriesServices.GetSubcategories();
    public Task<IResult> GetSubcategory(int id) => _subcategoriesServices.GetSubcategory(id);
    public Task<IResult> CreateSubcategory(Subcategory subcategory) => _subcategoriesServices.CreateSubcategory(subcategory);
    public Task<IResult> UpdateSubcategoryDetails(Subcategory update, int id) => _subcategoriesServices.UpdateSubcategoryDetails(update, id);
    public Task<IResult> RemoveSubcategory(int id) => _subcategoriesServices.RemoveSubcategory(id);
    #endregion
}
