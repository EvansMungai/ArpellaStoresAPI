using ArpellaStores.Features.InventoryManagement.Models;
using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;
using System.Globalization;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class ProductHelper : IProductHelper
{
    public List<Product> ParseCsv(Stream fileStream)
    {
        using var reader = new StreamReader(fileStream);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture);
        using var csv = new CsvReader(reader, config);
        return csv.GetRecords<Product>().ToList();
    }

    public List<Product> ParseExcel(Stream fileStream)
    {
        using var package = new ExcelPackage(fileStream);
        ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
        var rowcount = worksheet.Dimension.Rows;
        var products = new List<Product>();
        for (var row = 2; row <= rowcount; row++)
        {
            var product = new Product
            {
                InventoryId = worksheet.Cells[row, 1].Text,
                Name = worksheet.Cells[row, 2].Text,
                Price = decimal.Parse(worksheet.Cells[row, 3].Text),
                Category = int.Parse(worksheet.Cells[row, 4].Text),
                Subcategory = int.Parse(worksheet.Cells[row, 5].Text),
                Barcodes = worksheet.Cells[row, 6].Text,
                DiscountQuantity = int.Parse(worksheet.Cells[row, 7].Text),
                PurchaseCap = int.Parse(worksheet.Cells[row, 8].Text),
                PriceAfterDiscount = decimal.Parse(worksheet.Cells[row, 9].Text)
            };
            products.Add(product);
        }
        return products;
    }
}
