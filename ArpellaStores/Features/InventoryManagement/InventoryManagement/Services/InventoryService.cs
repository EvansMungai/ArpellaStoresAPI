using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _repo;
    private readonly IInventoryHelper _helper;
    public InventoryService(IInventoryRepository repo, IInventoryHelper helper)
    {
        _repo = repo;
        _helper = helper;
    }
    public async Task<IResult> GetInventories()
    {
        var inventories = await _repo.GetAllInventoriesAsync();
        return inventories == null || inventories.Count == 0 ? Results.NotFound("No inventories found") : Results.Ok(inventories);
    }
    public async Task<IResult> GetInventory(string id)
    {
        var inventory = await _repo.GetInventoryByIdAsync(id);
        return inventory == null ? Results.NotFound($"Inventory with ProductId = {id} was not found") : Results.Ok(inventory);
    }
    public async Task<IResult> CreateInventory(Inventory inventory)
    {
        var existing = await _repo.GetInventoryByIdAsync(inventory.ProductId);
        if (existing != null)
            return Results.Conflict($"An inventory with ProductID = {inventory.ProductId} already exists.");

        var newInventory = new Inventory
        {
            ProductId = inventory.ProductId,
            StockQuantity = inventory.StockQuantity,
            StockThreshold = inventory.StockThreshold,
            StockPrice = inventory.StockPrice,
            SupplierId = inventory.SupplierId,
            InvoiceNumber = inventory.InvoiceNumber
        };
        try
        {
            await _repo.AddInventoryAsync(newInventory);
            return Results.Ok(newInventory);
        }
        catch (Exception ex)
        {
            return Results.NotFound(ex.InnerException?.Message ?? ex.Message);
        }
    }
    public async Task<IResult> CreateInventories(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return Results.BadRequest("File is empty.");

            using var stream = file.OpenReadStream();
            // Parse and validate
            var (inventories, errors) = _helper.ParseExcel(stream);

            // Insert only valid records
            int insertedCount = 0;
            if (inventories.Any())
                insertedCount = await _repo.AddInventoriesAsync(inventories);

            // Return response
            return Results.Ok(new
            {
                message = insertedCount > 0
                    ? errors.Any()
                        ? "Upload completed with some validation errors."
                        : "Upload completed successfully."
                    : "Upload failed. No valid records were inserted.",
                insertedRecords = insertedCount,
                errors = errors
            });
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
    }
    public async Task<IResult> UpdateInventory(Inventory update, string id)
    {
        try
        {
            await _repo.UpdateInventoryDetails(update, id);
            Inventory? inventory = await _repo.GetInventoryByIdAsync(id);
            return Results.Ok(inventory);
        }
        catch (Exception ex) { return Results.NotFound(ex.InnerException?.Message ?? ex.Message); }
    }
    public async Task<IResult> RemoveInventory(string id)
    {
        var retrievedInventory = await _repo.GetInventoryByIdAsync(id);
        if (retrievedInventory == null) return Results.NotFound($"Inventory with ID = {id} was not found");

        try
        {
            await _repo.RemoveInventoryAsync(id);
            return Results.Ok(retrievedInventory);
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
    }
    #region Utilities
    public async Task<IResult> CheckInventoryLevels()
    {
        List<Inventory> lowStockItems = await _repo.CheckInventoryLevelsAsync();
        if (lowStockItems.Count != 0)
        {
            return Results.Ok(new
            {
                Message = "The following products are below the stock threshold",
                Items = lowStockItems.Select(item => new
                {
                    ProductId = item.ProductId,
                    StockQuantity = item.StockQuantity,
                    StockThreshold = item.StockThreshold
                })
            });
        }
        return Results.Ok("All inventory levels are above the stock threshold");
    }
    #endregion

    #region Restock Inventory
    public async Task<IResult> RestockInventory(Restocklog restocklog)
    {
        var newRestockLog = new Restocklog
        {
            ProductId = restocklog.ProductId,
            RestockQuantity = restocklog.RestockQuantity,
            RestockDate = DateTime.Now,
            SupplierId = restocklog.SupplierId,
            InvoiceNumber = restocklog.InvoiceNumber
        };
        try
        {
            await _repo.RestockInventoryAsync(restocklog);
            return Results.Ok(newRestockLog);
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
    }
    public async Task<IResult> GetRestockLogs()
    {
        var restockLogs = await _repo.GetAllRestocklogsAsync();
        return restockLogs == null || restockLogs.Count == 0 ? Results.NotFound("No Restock logs were found") : Results.Ok(restockLogs);
    }
    #endregion
}
