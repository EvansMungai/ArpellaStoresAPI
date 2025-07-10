using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class SupplierService : ISupplierService
{
    private readonly ArpellaContext _context;
    public SupplierService(ArpellaContext context)
    {
        _context = context;
    }
    public async Task<IResult> GetSuppliers()
    {
        var suppliers = await _context.Suppliers.Select(s => new { s.Id, s.SupplierName, s.KraPin }).AsNoTracking().ToListAsync();
        return suppliers == null || suppliers.Count == 0 ? Results.NotFound("No Suppliers were found") : Results.Ok(suppliers);
    }
    public async Task<IResult> GetSupplier(int id)
    {
        var supplier = await _context.Suppliers.Select(s => new { s.Id, s.SupplierName, s.KraPin }).AsNoTracking().SingleOrDefaultAsync(s => s.Id == id);
        return supplier == null ? Results.NotFound($"Supplier with id = {id} was not found") : Results.Ok(supplier);
    }
    public async Task<IResult> CreateSupplier(Supplier supplier)
    {
        var local = _context.Suppliers.Local.FirstOrDefault(s => s.Id == supplier.Id);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        var existing = await _context.Suppliers.AsNoTracking().SingleOrDefaultAsync(s => s.Id == supplier.Id);
        if (existing != null)
            return Results.Conflict($"An subcategory with ID = {supplier.Id} already exists.");

        var newSupplier = new Supplier
        {
            SupplierName = supplier.SupplierName,
            KraPin = supplier.KraPin,
        };
        try
        {
            _context.Suppliers.Add(newSupplier);
            await _context.SaveChangesAsync();
            return Results.Ok(newSupplier);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
    }
    public async Task<IResult> EditSupplierDetails(Supplier supplier, int id)
    {
        var local = _context.Suppliers.Local.FirstOrDefault(s => s.Id == id);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        var retrievedSupplier = await _context.Suppliers.SingleOrDefaultAsync(s => s.Id == id);

        if (retrievedSupplier != null)
        {
            retrievedSupplier.SupplierName = supplier.SupplierName;
            retrievedSupplier.KraPin = supplier.KraPin;
            try
            {
                _context.Suppliers.Update(retrievedSupplier);
                await _context.SaveChangesAsync();
                return Results.Ok(retrievedSupplier);
            }
            catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
        }
        else
        {
            return Results.NotFound($"Supplier with Id = {id} was not found");
        }
    }
    public async Task<IResult> RemoveSupplier(int id)
    {
        var local = _context.Suppliers.Local.FirstOrDefault(s => s.Id == id);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        var retrievedSupplier = await _context.Suppliers.SingleOrDefaultAsync(s => s.Id == id);

        if (retrievedSupplier != null)
        {
            try
            {
                _context.Suppliers.Remove(retrievedSupplier);
                await _context.SaveChangesAsync();
                return Results.Ok(retrievedSupplier);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
        else
        {
            return Results.NotFound($"Supplier with Id = {id} was not found");
        }
    }
}
