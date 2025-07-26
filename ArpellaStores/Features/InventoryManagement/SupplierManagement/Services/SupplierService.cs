using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _repo;
    public SupplierService(ISupplierRepository repo)
    {
        _repo = repo;
    }
    public async Task<IResult> GetSuppliers()
    {
        var suppliers = await _repo.GetAllSuppliersAsync();
        return suppliers == null || suppliers.Count == 0 ? Results.NotFound("No Suppliers were found") : Results.Ok(suppliers);
    }
    public async Task<IResult> GetSupplier(int id)
    {
        var supplier = await _repo.GetSupplierByIdAsync(id);
        return supplier == null ? Results.NotFound($"Supplier with id = {id} was not found") : Results.Ok(supplier);
    }
    public async Task<IResult> CreateSupplier(Supplier supplier)
    {
        var existing = await _repo.GetSupplierByIdAsync(supplier.Id);
        if (existing != null)
            return Results.Conflict($"An subcategory with ID = {supplier.Id} already exists.");

        try
        {
            await _repo.AddSupplierAsync(supplier);
            return Results.Ok(supplier);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
    }
    public async Task<IResult> EditSupplierDetails(Supplier update, int id)
    {
        try
        {
            bool updated = await _repo.UpdateSupplierDetailsAsync(update, id);
            if (!updated) return Results.NotFound($"Supplier with ID = {id} was not found");

            Supplier? supplier = await _repo.GetSupplierByIdAsync(id);
            return Results.Ok(supplier);
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
    }

    public async Task<IResult> RemoveSupplier(int id)
    {
        try
        {
            bool deleted = await _repo.RemoveSupplierAsync(id);
            if (!deleted)
                return Results.NotFound($"Supplier with ID ={id} was not found.");

            return Results.NoContent();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
    }
}

