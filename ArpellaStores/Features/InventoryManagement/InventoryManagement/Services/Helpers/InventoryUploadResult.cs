using ArpellaStores.Extensions;
using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class InventoryUploadResult
{
    public int InsertedCount => ValidProducts?.Count ?? 0;
    public int ErrorCount => ValidationErrors?.Count ?? 0;

    public List<Inventory> ValidProducts { get; set; } = new();
    public List<BulkModelValidator> ValidationErrors { get; set; } = new();
}
