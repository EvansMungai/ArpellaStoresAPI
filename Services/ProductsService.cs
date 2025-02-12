using ArpellaStores.Data;
using ArpellaStores.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Antiforgery;
using OfficeOpenXml;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace ArpellaStores.Services
{
    public class ProductsService : IProductsService
    {
        private readonly ArpellaContext _context;
        public ProductsService(ArpellaContext context)
        {
            _context = context;
        }
        public async Task<IResult> GetProducts()
        {
            var products = _context.Products.ToList();
            return products == null || products.Count == 0 ? Results.NotFound("No Products Found") : Results.Ok(products);
        }
        public async Task<IResult> GetProduct(string productId)
        {
            Product? product = _context.Products.SingleOrDefault(p => p.Id == productId);
            return product == null ? Results.NotFound($"Product with ProductID = {productId} was not found") : Results.Ok(product);
        }
        public async Task<IResult> CreateProduct(Product product)
        {
            var newProduct = new Product
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Category = product.Category
            };
            try
            {
                _context.Products.Add(newProduct);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { }
            return Results.Ok(newProduct);
        }
        //public async Task<IResult> CreateProducts(IFormFile file, IAntiforgery antiforgery, HttpContext context)
        //{
        //    try
        //    {
        //        await antiforgery.ValidateRequestAsync(context);
        //        if (file == null || file.Length == 0) return Results.BadRequest("File is empty");
        //        var products = file.FileName.EndsWith("csv") ? ParseCsv(file.OpenReadStream()) : ParseExcel(file.OpenReadStream());
        //        if (products == null || !products.Any())
        //        {
        //            return Results.NotFound("No valid data found in the file");
        //        }

        //        _context.Products.AddRangeAsync(products);
        //        await _context.SaveChangesAsync();
        //        return Results.Ok(products);

        //    }
        //    catch (AntiforgeryValidationException ex)
        //    {
        //        return Results.BadRequest(ex.Message);
        //    }
        //}
        public async Task<IResult> UpdateProductDetails(Product product, string id)
        {
            var retrievedProduct = _context.Products.FirstOrDefault(p => p.Id == id);
            if (retrievedProduct != null)
            {
                retrievedProduct.Id = product.Id;
                retrievedProduct.Name = product.Name;
                retrievedProduct.Price = product.Price;
                retrievedProduct.Category = product.Category;
                try
                {
                    _context.Update(retrievedProduct);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex) { }
                return Results.Ok(retrievedProduct);
            }
            else
            {
                return Results.NotFound($"Product with ProductID = {id} was not Found");
            }

        }
        public async Task<IResult> UpdateProductPrice(string id, decimal price)
        {
            var retrievedProduct = _context.Products.FirstOrDefault(p => p.Id == id);
            if (retrievedProduct != null)
            {
                retrievedProduct.PriceAfterDiscount = price;
                try
                {
                    _context.Update(retrievedProduct);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex) { }
                return Results.Ok(retrievedProduct);
            }
            else
            {
                return Results.NotFound($"Product with ProductID = {id} was not Found");
            }

        }
        public async Task<IResult> RemoveProduct(string productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                _context.Remove(product);
                await _context.SaveChangesAsync();
                return Results.Ok(product);
            }
            else
            {
                return Results.NotFound($"Product with ProductID = {productId} was not Found");
            }
        }
        #region Utilities
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
                    Id = worksheet.Cells[row, 1].Text,
                    Name = worksheet.Cells[row, 2].Text,
                    Price = decimal.Parse(worksheet.Cells[row, 3].Text),
                    Category = worksheet.Cells[row, 4].Text
                };
                products.Add(product);
            }
            return products;
        }
        #endregion
    }
}
