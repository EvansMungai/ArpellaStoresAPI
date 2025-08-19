using ArpellaStores.Features.InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class SubcategoriesService : ISubcategoriesServices
{
    private readonly ISubcategoryRepository _repo;
    public SubcategoriesService(ISubcategoryRepository repo)
    {
        _repo = repo;
    }
    public async Task<IResult> GetSubcategories()
    {
        var subcategories = await _repo.GetAllSubcategoriesAsync();
        return subcategories == null || subcategories.Count == 0 ? Results.NotFound("No Subcategories Found") : Results.Ok(subcategories);
    }
    public async Task<IResult> GetSubcategory(int id)
    {
        var retrievedSubcategory = await _repo.GetSubcategoryByIdAsync(id);
        return retrievedSubcategory == null ? Results.NotFound($"Subcategory of ID = {id} was not found") : Results.Ok(retrievedSubcategory);
    }
    public async Task<IResult> CreateSubcategory(Subcategory subcategory)
    {
        var existing = await _repo.GetSubcategoryByIdAsync(subcategory.Id);
        if (existing != null)
            return Results.Conflict($"An Subcategory with ID = {subcategory.Id} already exists.");

        try
        {
            await _repo.AddSubcategoryAsync(subcategory);
            return Results.Ok(subcategory);
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
    }
    public async Task<IResult> UpdateSubcategoryDetails(Subcategory update, int id)
    {
        try
        {
            await _repo.UpdateSubcategoryDetailsAsync(update, id);
            Subcategory? subcategory = await _repo.GetSubcategoryByIdAsync(id);
            return Results.Ok(subcategory);
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
    }
    public async Task<IResult> RemoveSubcategory(int id)
    {
        var subcategory = await _repo.GetSubcategoryByIdAsync(id);

        if (subcategory == null)
            return Results.NotFound($"Subcategory with SubcategoryId ={id} was not found");

        try
        {
            await _repo.RemoveSubcategoryAsync(id);
            return Results.Ok(subcategory);
        } catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
    }
}
