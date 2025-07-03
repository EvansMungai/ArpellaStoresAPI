using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class CategoriesService : ICategoriesService
{
    private readonly ArpellaContext _context;
    public CategoriesService(ArpellaContext context)
    {
        _context = context;
    }
    public async Task<IResult> GetCategories()
    {
        var categories = await _context.Categories.Select(c => new { c.Id, c.CategoryName }).ToListAsync();
        return categories == null || categories.Count == 0 ? Results.NotFound("No categories found") : Results.Ok(categories);
    }
    public async Task<IResult> GetCategory(int id)
    {
        var category = await _context.Categories.Select(c => new { c.Id, c.CategoryName }).SingleOrDefaultAsync(c => c.Id == id);
        return category == null ? Results.NotFound($"Category with CategoryID = {id} was not found") : Results.Ok(category);
    }
    public async Task<IResult> CreateCategory(Category category)
    {
        var newCategory = new Category
        {
            CategoryName = category.CategoryName
        };
        try
        {
            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();
            return Results.Ok(newCategory);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
    }
    public async Task<IResult> UpdateCategoryDetails(Category update, int id)
    {
        var retrievedCategory = await _context.Categories.SingleOrDefaultAsync(c => c.Id == id);
        if (retrievedCategory != null)
        {
            retrievedCategory.CategoryName = update.CategoryName;
            try
            {
                _context.Categories.Update(retrievedCategory);
                await _context.SaveChangesAsync();
                return Results.Ok(retrievedCategory);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
        else
        {
            return Results.NotFound($"Category with CategoryID = {id} was not found");
        }

    }
    public async Task<IResult> RemoveCategory(int categoryId)
    {
        var retrievedCategory = await _context.Categories.SingleOrDefaultAsync(c => c.Id == categoryId);
        if (retrievedCategory != null)
        {
            _context.Categories.Remove(retrievedCategory);
            await _context.SaveChangesAsync();
            return Results.Ok(retrievedCategory);
        }
        else
        {
            return Results.NotFound($"Category with CategoryID = {categoryId} was not found");
        }
    }
}
