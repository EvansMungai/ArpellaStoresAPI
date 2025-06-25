using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Services;

public interface ISupplierService
{
    Task<IResult> GetSuppliers();
    Task<IResult> GetSupplier(int id);
    Task<IResult> CreateSupplier(Supplier supplier);
    Task<IResult> EditSupplierDetails(Supplier supplier, int id);
    Task<IResult> RemoveSupplier(int id);
}
