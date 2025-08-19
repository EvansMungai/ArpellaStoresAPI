using ArpellaStores.Extensions;
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

    public (List<Inventory> Inventories, List<BulkModelValidator> Errors) ParseExcel(Stream fileStream)
    {
        using var package = new ExcelPackage(fileStream);
        ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
        var rowCount = worksheet.Dimension.Rows;

        var inventories = new List<Inventory>();
        var errors = new List<BulkModelValidator>();

        for (int row = 2; row <= rowCount; row++)
        {
            var rowErrors = new Dictionary<string, string>();
            var inventory = new Inventory();

            try
            {
                // ProductId
                var productId = worksheet.Cells[row, 1].Text?.Trim();
                if (string.IsNullOrEmpty(productId))
                    rowErrors["ProductId"] = "Product ID is required.";
                else
                    inventory.ProductId = productId;

                // StockQuantity
                var stockQtyText = worksheet.Cells[row, 2].Text?.Trim();
                if (string.IsNullOrEmpty(stockQtyText))
                    rowErrors["StockQuantity"] = "Stock quantity is required.";
                else if (!int.TryParse(stockQtyText, out var stockQty))
                    rowErrors["StockQuantity"] = "Stock quantity must be a valid integer.";
                else
                    inventory.StockQuantity = stockQty;

                // StockThreshold
                var thresholdText = worksheet.Cells[row, 3].Text?.Trim();
                if (string.IsNullOrEmpty(thresholdText))
                    rowErrors["StockThreshold"] = "Stock threshold is required.";
                else if (!int.TryParse(thresholdText, out var threshold))
                    rowErrors["StockThreshold"] = "Stock threshold must be a valid integer.";
                else
                    inventory.StockThreshold = threshold;

                // StockPrice
                var priceText = worksheet.Cells[row, 4].Text?.Trim();
                if (string.IsNullOrEmpty(priceText))
                    rowErrors["StockPrice"] = "Stock price is required.";
                else if (!decimal.TryParse(priceText, out var price))
                    rowErrors["StockPrice"] = "Stock price must be a valid decimal.";
                else
                    inventory.StockPrice = price;

                // SupplierId
                var supplierIdText = worksheet.Cells[row, 5].Text?.Trim();
                if (string.IsNullOrEmpty(supplierIdText))
                    rowErrors["SupplierId"] = "Supplier ID is required.";
                else if (!int.TryParse(supplierIdText, out var supplierId))
                    rowErrors["SupplierId"] = "Supplier ID must be a valid integer.";
                else
                    inventory.SupplierId = supplierId;

                // InvoiceNumber
                var invoiceNumber = worksheet.Cells[row, 6].Text?.Trim();
                if (string.IsNullOrEmpty(invoiceNumber))
                    rowErrors["InvoiceNumber"] = "Invoice number is required.";
                else
                    inventory.InvoiceNumber = invoiceNumber;

                // Model-level validation
                if (!rowErrors.Any())
                {
                    var modelErrors = BulkUploadValidator<Inventory>.Validate(inventory);
                    foreach (var kvp in modelErrors)
                    {
                        rowErrors[kvp.Key] = string.Join("; ", kvp.Value);
                    }
                }
            }
            catch (Exception ex)  { rowErrors["ParsingError"] = $"Unexpected error: {ex.Message}"; }

            if (rowErrors.Any())
            {
                errors.Add(new BulkModelValidator
                {
                    RowNumber = row,
                    Errors = rowErrors.ToDictionary(
                        kvp => kvp.Key,
                        kvp => new List<string> { kvp.Value }
                    )
                });
            }
            else { inventories.Add(inventory); }
        }
        return (inventories, errors);
    }
}
