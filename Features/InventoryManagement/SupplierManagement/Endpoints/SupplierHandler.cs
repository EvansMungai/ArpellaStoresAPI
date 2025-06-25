using ArpellaStores.Extensions;
using ArpellaStores.Features.InventoryManagement.Models;
using ArpellaStores.Features.InventoryManagement.Services;

namespace ArpellaStores.Features.InventoryManagement.Endpoints;

public class SupplierHandler : IHandler
{
    public static string RouteName => "Supplier Management";
    private readonly ISupplierService _supplierService;
    public SupplierHandler(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    public Task<IResult> GetSuppliers() => _supplierService.GetSuppliers();
    public Task<IResult> GetSupplier(int id) => _supplierService.GetSupplier(id);
    public Task<IResult> CreateSupplier(Supplier supplier) => _supplierService.CreateSupplier(supplier);
    public Task<IResult> EditSupplierDetails(Supplier supplier, int id) => _supplierService.EditSupplierDetails(supplier, id);
    public Task<IResult> RemoveSupplier(int id) => _supplierService.RemoveSupplier(id);
}
