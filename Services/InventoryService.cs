using ArpellaStores.Data;
using ArpellaStores.Models;

namespace ArpellaStores.Services;

public class InventoryService : IInventoryService
{
    private readonly ArpellaContext _context;
    public InventoryService(ArpellaContext context)
    {
        _context = context;
    }
    public async Task<IResult> GetInventories()
    {
        var inventories = _context.Inventories.Select(i => new { i.ProductId, i.StockQuantity, i.StockThreshold, i.StockPrice, i.CreatedAt, i.UpdatedAt }).ToList();
        return inventories == null || inventories.Count == 0 ? Results.NotFound("No inventories found") : Results.Ok(inventories);
    }
    public async Task<IResult> GetInventory(string id)
    {
        var inventory = _context.Inventories.Where(i => i.ProductId == id).Select(i => new { i.ProductId, i.StockQuantity, i.StockThreshold, i.StockPrice, i.CreatedAt, i.UpdatedAt }).SingleOrDefault();
        return inventory == null ? Results.NotFound($"Inventory with InventoryId = {id} was not found") : Results.Ok(inventory);
    }
    public async Task<IResult> CreateInventory(Inventory inventory)
    {
        var newInventory = new Inventory
        {
            ProductId = inventory.ProductId,
            StockQuantity = inventory.StockQuantity,
            StockThreshold = inventory.StockThreshold
        };
        try
        {
            _context.Inventories.Add(newInventory);
            await _context.SaveChangesAsync();
            return Results.Ok(newInventory);
        }
        catch (Exception ex)
        {
            return Results.NotFound(ex.Message);
        }
    }
    public async Task<IResult> UpdateInventory(Inventory update, string id)
    {
        Inventory? retrievedInventory = _context.Inventories.FirstOrDefault(i => i.ProductId == id);
        if (retrievedInventory != null)
        {
            retrievedInventory.ProductId = update.ProductId;
            retrievedInventory.StockQuantity = update.StockQuantity;
            retrievedInventory.StockThreshold = update.StockThreshold;
            retrievedInventory.UpdatedAt = DateTime.Now;
            try
            {
                _context.Inventories.Update(retrievedInventory);
                await _context.SaveChangesAsync();
                return Results.Ok(retrievedInventory);
            }
            catch (Exception ex)
            {
                return Results.NotFound(ex.Message);
            }
        }
        else
        {
            return Results.NotFound($"Inventory with InventoryID = {id} was not found");
        }
    }
    public async Task<IResult> RemoveInventory(string id)
    {
        Inventory? retrievedInventory = _context.Inventories.FirstOrDefault(i => i.ProductId == id);
        if (retrievedInventory != null)
        {
            try
            {
                _context.Inventories.Remove(retrievedInventory);
                await _context.SaveChangesAsync();
                return Results.Ok(retrievedInventory);
            }
            catch (Exception ex)
            {
                return Results.Problem("Exception: " + ex.Message);
            }

        }
        else { return Results.NotFound($"Inventory with id = {id} was not found"); }
    }
}
