using ArpellaStores.Extensions.RouteHandlers;
using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Endpoints;

public class InvoiceRoutes : IRouteRegistrar
{
    public void RegisterRoutes(WebApplication app)
    {
        MapInvoiceRoutes(app);
    }
    public void MapInvoiceRoutes(WebApplication webApplication)
    {
        var app = webApplication.MapGroup("").WithTags("Invoices");
        app.MapGet("/invoices", (InvoiceHandler handler) => handler.GetInvoices()).Produces(200).Produces(404).Produces<List<Invoice>>();
        app.MapGet("/invoice/{id}", (InvoiceHandler handler, string id) => handler.GetInvoice(id)).Produces(200).Produces(404).Produces<Invoice>();
        app.MapPost("/invoice", (InvoiceHandler handler, Invoice invoice) => handler.CreateInvoice(invoice)).Produces(200).Produces(404).Produces<Invoice>();
        app.MapPut("/invoice/{id}", (InvoiceHandler handler, Invoice invoice, string id) => handler.UpdateInvoiceDetails(invoice, id)).Produces(200).Produces(404).Produces<Invoice>();
        app.MapDelete("/invoice/{id}", (InvoiceHandler handler, string id) => handler.RemoveInvoice(id)).Produces(200).Produces(404).Produces<Invoice>();
    }
}
