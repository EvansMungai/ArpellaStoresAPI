using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class SubcategoriesService : ISubcategoriesServices
{
    private readonly ArpellaContext _context;
    public SubcategoriesService(ArpellaContext context)
    {
        _context = context;
    }
    public async Task<IResult> GetSubcategories()
    {
        var subcategories = await _context.Subcategories.Select(s => new { s.Id, s.SubcategoryName, s.CategoryId }).AsNoTracking().ToListAsync();
        return subcategories == null || subcategories.Count == 0 ? Results.NotFound("No Subcategories Found") : Results.Ok(subcategories);
    }
    public async Task<IResult> GetSubcategory(int id)
    {
        var retrievedSubcategory = await _context.Subcategories.Select(s => new { s.Id, s.SubcategoryName, s.CategoryId }).AsNoTracking().SingleOrDefaultAsync(s => s.Id == id);
        return retrievedSubcategory == null ? Results.NotFound($"Subcategory of ID = {id} was not found") : Results.Ok(retrievedSubcategory);
    }
    public async Task<IResult> CreateSubcategory(Subcategory subcategory)
    {
        var local = _context.Subcategories.Local.FirstOrDefault(s => s.Id == subcategory.Id);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }
        var existing = await _context.Subcategories.AsNoTracking().SingleOrDefaultAsync(s => s.Id == subcategory.Id);
        if (existing != null)
            return Results.Conflict($"An Subcategory with ID = {subcategory.Id} already exists.");

        var newSubcategory = new Subcategory
        {
            SubcategoryName = subcategory.SubcategoryName,
            CategoryId = subcategory.CategoryId
        };
        try
        {
            _context.Subcategories.Add(newSubcategory);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }

        return Results.Ok(newSubcategory);
    }
    public async Task<IResult> UpdateSubcategoryDetails(Subcategory update, int id)
    {
        var local = _context.Subcategories.Local.FirstOrDefault(s => s.Id == id);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        var retrievedCategory = await _context.Subcategories.SingleOrDefaultAsync(s => s.Id == id);
        if (retrievedCategory != null)
        {
            retrievedCategory.SubcategoryName = update.SubcategoryName;
            retrievedCategory.CategoryId = update.CategoryId;
            try
            {
                _context.Subcategories.Update(retrievedCategory);
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
            return Results.NotFound($"Subcategory with ID = {id} was not found");
        }

    }
    public async Task<IResult> RemoveSubcategory(int id)
    {
        var local = _context.Subcategories.Local.FirstOrDefault(s => s.Id == id);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        var subcategory = await _context.Subcategories.SingleOrDefaultAsync(s => s.Id == id);

        if (subcategory != null)
        {
            _context.Subcategories.Remove(subcategory);
            await _context.SaveChangesAsync();
            return Results.Ok(subcategory);
        }
        else
        {
            return Results.NotFound($"Category with CategoryID = {id} was not found");
        }
    }
}
