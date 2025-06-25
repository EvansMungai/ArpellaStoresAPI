using ArpellaStores.Extensions;
using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Endpoints;

public class SupplierRoutes : IRouteRegistrar
{
    public void RegisterRoutes(WebApplication app)
    {
        MapSupplierRoutes(app);
    }
    public void MapSupplierRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Suppliers");
        app.MapGet("/suppliers", (SupplierHandler handler) => handler.GetSuppliers()).Produces(200).Produces(404).Produces<List<Supplier>>();
        app.MapGet("/supplier/{id}", (SupplierHandler handler,int id) => handler.GetSupplier(id)).Produces(200).Produces(404).Produces<Supplier>();
        app.MapPost("/supplier", (SupplierHandler handler, Supplier supplier) => handler.CreateSupplier(supplier)).Produces<Supplier>();
        app.MapPut("/supplier/{id}", (SupplierHandler handler, Supplier supplier, int id) => handler.EditSupplierDetails(supplier, id)).Produces<Supplier>();
        app.MapDelete("/supplier/{id}", (SupplierHandler handler, int id) => handler.RemoveSupplier(id)).Produces(200).Produces(404).Produces<Supplier>();
    }
}
