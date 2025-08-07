using ArpellaStores.Extensions;
using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class ProductUploadResult
{
    public int InsertedCount => ValidProducts?.Count ?? 0;
    public int ErrorCount => ValidationErrors?.Count ?? 0;

    public List<Product> ValidProducts { get; set; } = new List<Product>();
    public List<BulkModelValidator> ValidationErrors { get; set; } = new List<BulkModelValidator>();
}
