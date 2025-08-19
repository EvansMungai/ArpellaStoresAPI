using ArpellaStores.Extensions;
using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Services;

public interface IProductHelper
{
    List<Product> ParseCsv(Stream fileStream);
    (List<Product> Products, List<BulkModelValidator> Errors) ParseExcel(Stream fileStream);
}
