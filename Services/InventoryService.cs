using ArpellaStores.Data;
using ArpellaStores.Models;
using CsvHelper.Configuration;
using CsvHelper;
using OfficeOpenXml;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

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
            StockThreshold = inventory.StockThreshold,
            StockPrice = inventory.StockPrice
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
    public async Task<IResult> CreateInventories(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0) return Results.BadRequest("File is empty");
            var inventory = file.FileName.EndsWith("csv") ? ParseCsv(file.OpenReadStream()) : ParseExcel(file.OpenReadStream());
            if (inventory == null || !inventory.Any())
            {
                return Results.NotFound("No valid data found in the file");
            }

            _context.Inventories.AddRangeAsync(inventory);
            await _context.SaveChangesAsync();
            return Results.Ok(inventory);

        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.InnerException.Message);
        }
    }
    public async Task<IResult> UpdateInventory(Inventory update, string id)
    {
        Inventory? retrievedInventory = _context.Inventories.FirstOrDefault(i => i.ProductId == id);
        if (retrievedInventory != null)
        {
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
    #region Utilities
    public async Task<IResult> CheckInventoryLevels()
    {
        var lowStockItems = _context.Inventories.Where(i => i.StockQuantity <= i.StockThreshold).ToList();
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
    public List<Inventory> ParseCsv(Stream fileStream)
    {
        using var reader = new StreamReader(fileStream);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture);
        using var csv = new CsvReader(reader, config);
        return csv.GetRecords<Inventory>().ToList();
    }
    public List<Inventory> ParseExcel(Stream fileStream)
    {
        using var package = new ExcelPackage(fileStream);
        ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
        var rowcount = worksheet.Dimension.Rows;
        var inventories = new List<Inventory>();
        for (var row = 2; row <= rowcount; row++)
        {
            var inventory = new Inventory
            {
                ProductId = worksheet.Cells[row, 1].Text,
                StockQuantity = int.Parse(worksheet.Cells[row, 2].Text),
                StockThreshold = int.Parse(worksheet.Cells[row, 3].Text),
                StockPrice = decimal.Parse(worksheet.Cells[row, 4].Text)
            };
            inventories.Add(inventory);
        }
        return inventories;
    }
    #endregion
}
