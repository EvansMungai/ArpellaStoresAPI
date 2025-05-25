using ArpellaStores.Extensions;
using ArpellaStores.Models;
using ArpellaStores.Services;

namespace ArpellaStores.Features.Inventory_and_Product_management.Supplier_Management.Endpoints;

public class SupplierRoutes : IRouteRegistrar
{
    private readonly ISupplierService _supplierService;
    public SupplierRoutes(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }
    public void RegisterRoutes(WebApplication app)
    {
        MapSupplierRoutes(app);
    }
    public void MapSupplierRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Suppliers");
        app.MapGet("/suppliers", () => this._supplierService.GetSuppliers()).Produces(200).Produces(404).Produces<List<Supplier>>();
        app.MapGet("/supplier/{id}", (int id) => this._supplierService.GetSupplier(id)).Produces(200).Produces(404).Produces<Supplier>();
        app.MapPost("/supplier", (Supplier supplier) => this._supplierService.CreateSupplier(supplier)).Produces<Supplier>();
        app.MapPut("/supplier/{id}", (Supplier supplier, int id) => this._supplierService.EditSupplierDetails(supplier, id)).Produces<Supplier>();
        app.MapDelete("/supplier/{id}", (int id) => this._supplierService.RemoveSupplier(id)).Produces(200).Produces(404).Produces<Supplier>();
    }
}
