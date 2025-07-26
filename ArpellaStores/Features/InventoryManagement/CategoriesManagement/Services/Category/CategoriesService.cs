using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class CategoriesService : ICategoriesService
{
    private readonly ICategoryRepository _repo;
    public CategoriesService(ICategoryRepository repo)
    {
        _repo = repo;
    }
    public async Task<IResult> GetCategories()
    {
        var categories = await _repo.GetAllCategoriesAsync();
        return categories == null || categories.Count == 0 ? Results.NotFound("No categories found") : Results.Ok(categories);
    }
    public async Task<IResult> GetCategory(int id)
    {
        var category = await _repo.GetCategoryByIdAsync(id);
        return category == null ? Results.NotFound($"Category with CategoryID = {id} was not found") : Results.Ok(category);
    }
    public async Task<IResult> CreateCategory(Category category)
    {
        var existing = await _repo.GetCategoryByIdAsync(category.Id);
        if (existing != null)
            return Results.Conflict($"An category with ID = {category.Id} already exists.");

        try
        {
            await _repo.AddCategoryAsync(category);
            return Results.Ok(category);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
    }
    public async Task<IResult> UpdateCategoryDetails(Category update, int id)
    {
        try
        {
            await _repo.UpdateCategoryDetailsAsync(update, id);
            Category? updatedCategory = await _repo.GetCategoryByIdAsync(id);
            return Results.Ok(updatedCategory);
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }

    }
    public async Task<IResult> RemoveCategory(int categoryId)
    {
        var retrievedCategory = await _repo.GetCategoryByIdAsync(categoryId);
        if (retrievedCategory == null)
            return Results.NotFound($"Category with CategoryID = {categoryId} was not found");

        try
        {
            await _repo.RemoveCategoryAsync(categoryId);
            return Results.Ok(retrievedCategory);
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
    }
}
