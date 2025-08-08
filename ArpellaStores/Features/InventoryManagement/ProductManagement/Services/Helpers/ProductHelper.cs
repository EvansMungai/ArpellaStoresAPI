using ArpellaStores.Extensions;
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

    public (List<Product> Products, List<BulkModelValidator> Errors) ParseExcel(Stream fileStream)
    {
        using var package = new ExcelPackage(fileStream);
        ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
        var rowcount = worksheet.Dimension.Rows;

        var products = new List<Product>();
        var errors = new List<BulkModelValidator>();

        for (var row = 2; row <= rowcount; row++)
        {
            var rowErrors = new Dictionary<string, string>();
            var product = new Product();

            // InventoryId
            var inventoryId = worksheet.Cells[row, 1].Text?.Trim();
            if (string.IsNullOrEmpty(inventoryId))
                rowErrors["InventoryId"] = "Inventory ID is required.";
            else
                product.InventoryId = inventoryId;

            // Name
            var name = worksheet.Cells[row, 2].Text?.Trim();
            if (string.IsNullOrEmpty(name))
                rowErrors["Name"] = "Product name is required.";
            else
                product.Name = name;

            // Price
            var priceText = worksheet.Cells[row, 3].Text?.Trim();
            if (string.IsNullOrEmpty(priceText))
                rowErrors["Price"] = "Price is required.";
            else if (!decimal.TryParse(priceText, out var price))
                rowErrors["Price"] = "Price must be a valid decimal.";
            else
                product.Price = price;

            // Category
            var categoryText = worksheet.Cells[row, 4].Text?.Trim();
            if (string.IsNullOrEmpty(categoryText))
                rowErrors["Category"] = "Category is required.";
            else if (!int.TryParse(categoryText, out var category))
                rowErrors["Category"] = "Category must be a valid integer.";
            else
                product.Category = category;

            // Subcategory
            var subcategoryText = worksheet.Cells[row, 5].Text?.Trim();
            if (string.IsNullOrEmpty(subcategoryText))
                rowErrors["Subcategory"] = "Subcategory is required.";
            else if (!int.TryParse(subcategoryText, out var subcategory))
                rowErrors["Subcategory"] = "Subcategory must be a valid integer.";
            else
                product.Subcategory = subcategory;

            // DiscountQuantity
            var discountQtyText = worksheet.Cells[row, 7].Text?.Trim();
            int discountQty = int.TryParse(discountQtyText, out var result) ? result : 0;
            product.DiscountQuantity = discountQty;

            // PurchaseCap
            var purchaseCapText = worksheet.Cells[row, 8].Text?.Trim();
            if (string.IsNullOrEmpty(purchaseCapText))
                rowErrors["PurchaseCap"] = "Purchase cap is required.";
            else if (!int.TryParse(purchaseCapText, out var purchaseCap))
                rowErrors["PurchaseCap"] = "Purchase cap must be a valid integer.";
            else
                product.PurchaseCap = purchaseCap;

            // PriceAfterDiscount
            var priceAfterDiscountText = worksheet.Cells[row, 9].Text?.Trim();
            if (string.IsNullOrEmpty(priceAfterDiscountText))
                rowErrors["PriceAfterDiscount"] = "Price after discount is required.";
            else if (!decimal.TryParse(priceAfterDiscountText, out var priceAfterDiscount))
                rowErrors["PriceAfterDiscount"] = "Price after discount must be a valid decimal.";
            else
                product.PriceAfterDiscount = priceAfterDiscount;

            // Run model-level validation only if pre-validation passed
            if (!rowErrors.Any())
            {
                var modelErrors = BulkUploadValidator<Product>.Validate(product);
                foreach (var kvp in modelErrors)
                {
                    rowErrors[kvp.Key] = string.Join("; ", kvp.Value);
                }
            }

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
            else
            {
                products.Add(product);
            }
        }

        return (products, errors);
    }
}
