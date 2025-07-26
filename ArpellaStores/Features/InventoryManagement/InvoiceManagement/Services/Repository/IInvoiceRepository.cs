using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Services;

public interface IInvoiceRepository
{
    Task<List<Invoice>> GetAllInvoicesAsync();
    Task<Invoice?> GetInvoiceByIdAsync(string id);
    Task AddInvoice(Invoice invoice);
    Task UpdateInvoiceDetailsAsync(Invoice update, string id);
    Task RemoveInvoiceAsync(string id);
}
