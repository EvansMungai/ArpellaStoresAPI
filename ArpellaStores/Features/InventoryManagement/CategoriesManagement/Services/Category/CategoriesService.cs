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
        var categories = await _context.Categories.Select(c => new { c.Id, c.CategoryName }).AsNoTracking().ToListAsync();
        return categories == null || categories.Count == 0 ? Results.NotFound("No categories found") : Results.Ok(categories);
    }
    public async Task<IResult> GetCategory(int id)
    {
        var category = await _context.Categories.Select(c => new { c.Id, c.CategoryName }).AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);
        return category == null ? Results.NotFound($"Category with CategoryID = {id} was not found") : Results.Ok(category);
    }
    public async Task<IResult> CreateCategory(Category category)
    {
        var local = _context.Categories.Local.FirstOrDefault(c => c.Id == category.Id);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        var existing = await _context.Categories.AsNoTracking().SingleOrDefaultAsync(c => c.Id == category.Id);
        if (existing != null)
            return Results.Conflict($"An category with ID = {category.Id} already exists.");

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
        var local = _context.Categories.Local.FirstOrDefault(c => c.Id == id);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

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
        var local = _context.Categories.Local.FirstOrDefault(c => c.Id == categoryId);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

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
