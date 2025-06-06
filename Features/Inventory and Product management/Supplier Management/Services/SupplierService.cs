using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Models;

namespace ArpellaStores.Services;

public class SupplierService : ISupplierService
{
    private readonly ArpellaContext _context;
    public SupplierService(ArpellaContext context)
    {
        _context = context;
    }
    public async Task<IResult> GetSuppliers()
    {
        var suppliers = _context.Suppliers.Select(s => new { s.Id, s.SupplierName, s.KraPin }).ToList();
        return suppliers == null || suppliers.Count == 0 ? Results.NotFound("No Suppliers were found") : Results.Ok(suppliers);
    }
    public async Task<IResult> GetSupplier(int id)
    {
        var supplier = _context.Suppliers.Select(s => new { s.Id, s.SupplierName, s.KraPin }).SingleOrDefault(s => s.Id == id);
        return supplier == null ? Results.NotFound($"Supplier with id = {id} was not found") : Results.Ok(supplier);
    }
    public async Task<IResult> CreateSupplier(Supplier supplier)
    {
        var newSupplier = new Supplier
        {
            SupplierName = supplier.SupplierName,
            KraPin = supplier.KraPin,
        };
        try
        {
            _context.Suppliers.Add(newSupplier);
            _context.SaveChangesAsync();
            return Results.Ok(newSupplier);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.InnerException.Message);
        }
    }
    public async Task<IResult> EditSupplierDetails(Supplier supplier, int id)
    {
        var retrievedSupplier = _context.Suppliers.SingleOrDefault(s => s.Id == id);
        if (retrievedSupplier != null)
        {
            retrievedSupplier.SupplierName = supplier.SupplierName;
            retrievedSupplier.KraPin = supplier.KraPin;
            try
            {
                _context.Suppliers.Update(retrievedSupplier);
                _context.SaveChangesAsync();
                return Results.Ok(retrievedSupplier);
            }
            catch (Exception ex) { return Results.BadRequest(ex.InnerException.Message); }
        }
        else
        {
            return Results.NotFound($"Supplier with Id = {id} was not found");
        }
    }
    public async Task<IResult> RemoveSupplier(int id)
    {
        var retrievedSupplier = _context.Suppliers.SingleOrDefault(s => s.Id == id);
        if (retrievedSupplier != null)
        {
            try
            {
                _context.Suppliers.Update(retrievedSupplier);
                _context.SaveChangesAsync();
                return Results.Ok(retrievedSupplier);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.InnerException?.Message);
            }
        }
        else
        {
            return Results.NotFound($"Supplier with Id = {id} was not found");
        }
    }
}
