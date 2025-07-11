﻿using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.InventoryManagement.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Globalization;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class InventoryService : IInventoryService
{
    private readonly ArpellaContext _context;
    public InventoryService(ArpellaContext context)
    {
        _context = context;
    }
    public async Task<IResult> GetInventories()
    {
        var inventories = await _context.Inventories.Select(i => new { i.InventoryId, i.ProductId, i.StockQuantity, i.StockThreshold, i.StockPrice, i.SupplierId, i.CreatedAt, i.UpdatedAt }).AsNoTracking().ToListAsync();
        return inventories == null || inventories.Count == 0 ? Results.NotFound("No inventories found") : Results.Ok(inventories);
    }
    public async Task<IResult> GetInventory(string id)
    {
        var inventory = await _context.Inventories.Where(i => i.ProductId == id).Select(i => new { i.InventoryId, i.ProductId, i.StockQuantity, i.StockThreshold, i.StockPrice, i.SupplierId, i.CreatedAt, i.UpdatedAt }).AsNoTracking().SingleOrDefaultAsync();
        return inventory == null ? Results.NotFound($"Inventory with ProductId = {id} was not found") : Results.Ok(inventory);
    }
    public async Task<IResult> CreateInventory(Inventory inventory)
    {
        var local = _context.Inventories.Local.FirstOrDefault(i => i.ProductId == inventory.ProductId);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        var existing = await _context.Inventories.AsNoTracking().SingleOrDefaultAsync(i => i.ProductId == inventory.ProductId);
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
            _context.Inventories.Add(newInventory);
            await _context.SaveChangesAsync();
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
            if (file == null || file.Length == 0) return Results.BadRequest("File is empty");
            var inventory = file.FileName.EndsWith("csv") ? ParseCsv(file.OpenReadStream()) : ParseExcel(file.OpenReadStream());
            if (inventory == null || inventory.Count == 0)
            {
                return Results.NotFound("No valid data found in the file");
            }

            _context.Inventories.AddRangeAsync(inventory);
            await _context.SaveChangesAsync();
            return Results.Ok(inventory);

        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
    }
    public async Task<IResult> UpdateInventory(Inventory update, string id)
    {
        var local =  _context.Inventories.Local.FirstOrDefault(i => i.ProductId == id);

        if (local != null)
            _context.Entry(local).State = EntityState.Detached;

        Inventory? retrievedInventory = await _context.Inventories.AsNoTracking().SingleOrDefaultAsync(i => i.ProductId == id);
        if (retrievedInventory != null)
        {
            retrievedInventory.StockQuantity = update.StockQuantity;
            retrievedInventory.StockThreshold = update.StockThreshold;
            retrievedInventory.UpdatedAt = DateTime.Now;
            retrievedInventory.SupplierId = update.SupplierId;
            try
            {
                _context.Inventories.Update(retrievedInventory);
                await _context.SaveChangesAsync();
                return Results.Ok(retrievedInventory);
            }
            catch (Exception ex)
            {
                return Results.NotFound(ex.InnerException?.Message ?? ex.Message);
            }
        }
        else
        {
            return Results.NotFound($"Inventory with ProductID = {id} was not found");
        }
    }
    public async Task<IResult> RemoveInventory(string id)
    {
        var local = _context.Inventories.Local.FirstOrDefault(i => i.ProductId == id);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        Inventory? retrievedInventory = await _context.Inventories.AsNoTracking().SingleOrDefaultAsync(i => i.ProductId == id);
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
                return Results.Problem("Exception: " + ex.InnerException?.Message ?? ex.Message);
            }

        }
        else { return Results.NotFound($"Inventory with id = {id} was not found"); }
    }
    #region Utilities
    public async Task<IResult> CheckInventoryLevels()
    {
        var lowStockItems = await _context.Inventories.Where(i => i.StockQuantity <= i.StockThreshold).ToListAsync();
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
                StockPrice = decimal.Parse(worksheet.Cells[row, 4].Text),
                SupplierId = int.Parse(worksheet.Cells[row, 5].Text),
                InvoiceNumber = worksheet.Cells[row, 6].Text
            };
            inventories.Add(inventory);
        }
        return inventories;
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
            Inventory? inventory = await _context.Inventories.SingleOrDefaultAsync(i => i.ProductId == restocklog.ProductId);
            if (inventory == null)
            {
                return Results.BadRequest($"Inventory with productid ={restocklog.ProductId} was not found");
            }
            inventory.StockQuantity += restocklog.RestockQuantity;
            _context.Restocklogs.Add(newRestockLog);
            _context.SaveChangesAsync();
            return Results.Ok(newRestockLog);
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
    }
    public async Task<IResult> GetRestockLogs()
    {
        var restockLogs = await _context.Restocklogs.Select(l => new { l.LogId, l.ProductId, l.RestockQuantity, l.RestockDate, l.SupplierId, l.InvoiceNumber }).ToListAsync();
        return restockLogs == null || restockLogs.Count == 0 ? Results.NotFound("No Restock logs were found") : Results.Ok(restockLogs);
    }
    #endregion
}
