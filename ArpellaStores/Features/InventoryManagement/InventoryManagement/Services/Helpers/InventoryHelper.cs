using ArpellaStores.Features.InventoryManagement.Models;
using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;
using System.Globalization;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class InventoryHelper : IInventoryHelper
{
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
}
