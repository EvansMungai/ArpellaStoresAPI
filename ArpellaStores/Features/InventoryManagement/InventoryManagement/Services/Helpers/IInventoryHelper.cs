using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Services;

public interface IInventoryHelper
{
    List<Inventory> ParseCsv(Stream fileStream);
    List<Inventory> ParseExcel(Stream fileStream);
}
