using ArpellaStores.Extensions;
using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Services;

public interface IInventoryHelper
{
    List<Inventory> ParseCsv(Stream fileStream);
    (List<Inventory> Inventories, List<BulkModelValidator> Errors) ParseExcel(Stream fileStream);
}
