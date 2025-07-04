using ArpellaStores.Data.Infrastructure;
using ArpellaStores.Features.InventoryManagement.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Globalization;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class ProductsService : IProductsService
{
    private readonly ArpellaContext _context;
    private readonly ICloudinaryService _cloudinaryService;
    public ProductsService(ArpellaContext context, ICloudinaryService cloudinaryService)
    {
        _context = context;
        _cloudinaryService = cloudinaryService;
    }
    public async Task<IResult> GetProducts()
    {
        var products = await _context.Products.Select(p => new { p.Id, p.InventoryId, p.Name, p.Price, p.Category, p.PurchaseCap, p.Subcategory, p.Barcodes, p.TaxCode, p.CreatedAt, p.UpdatedAt }).AsNoTracking().ToListAsync();
        return products == null || products.Count == 0 ? Results.NotFound("No Products Found") : Results.Ok(products);
    }
    public async Task<IResult> GetProduct(int productId)
    {
        var product = await _context.Products.Select(p => new { p.Id, p.InventoryId, p.Name, p.Price, p.Category, p.PurchaseCap, p.Subcategory, p.Barcodes, p.TaxCode, p.CreatedAt, p.UpdatedAt }).AsNoTracking().SingleOrDefaultAsync(p => p.Id == productId);
        return product == null ? Results.NotFound($"Product with ProductID = {productId} was not found") : Results.Ok(product);
    }
    public async Task<IResult> CreateProduct(Product product)
    {
        var local = _context.Products.Local.FirstOrDefault(p => p.Id == product.Id);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        var existing = await _context.Products.AsNoTracking().SingleOrDefaultAsync(p => p.Id == product.Id);
        if (existing != null)
            return Results.Conflict($"An Product with ProductID = {product.Id} already exists.");

        var newProduct = new Product
        {
            InventoryId = product.InventoryId,
            Name = product.Name,
            Price = product.Price,
            PriceAfterDiscount = product.PriceAfterDiscount,
            Category = product.Category,
            Subcategory = product.Subcategory,
            DiscountQuantity = product.DiscountQuantity,
            Barcodes = product.Barcodes,
            PurchaseCap = product.PurchaseCap,
            TaxCode = product.TaxCode
        };
        try
        {
            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();
            return Results.Ok(newProduct);
        }
        catch (Exception ex)
        {
            return Results.NotFound(ex.InnerException?.Message ?? ex.Message);
        }
    }
    public async Task<IResult> CreateProducts(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0) return Results.BadRequest("File is empty");
            var isCsv = file.ContentType == "text/csv" || file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase);

            using var stream = file.OpenReadStream();
            var products = isCsv ? ParseCsv(stream) : ParseExcel(stream);
            if (products == null || products.Count == 0) return Results.NotFound("No valid data found in the file");

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
            return Results.Ok(products);

        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
    }
    public async Task<IResult> UpdateProductDetails(Product product, int id)
    {
        var local = _context.Products.Local.FirstOrDefault(p => p.Id == product.Id);

        if (local != null)
            _context.Entry(local).State = EntityState.Detached;

        var retrievedProduct = await _context.Products.AsNoTracking().SingleOrDefaultAsync(p => p.Id == id);
        if (retrievedProduct != null)
        {
            retrievedProduct.Id = product.Id;
            retrievedProduct.Name = product.Name;
            retrievedProduct.Price = product.Price;
            retrievedProduct.Category = product.Category;
            retrievedProduct.Subcategory = product.Subcategory;
            retrievedProduct.DiscountQuantity = product.DiscountQuantity;
            retrievedProduct.Barcodes = product.Barcodes;
            retrievedProduct.PurchaseCap = product.PurchaseCap;
            retrievedProduct.PriceAfterDiscount = product.PriceAfterDiscount;
            try
            {
                _context.Update(retrievedProduct);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
            return Results.Ok(retrievedProduct);
        }
        else
        {
            return Results.NotFound($"Product with ProductID = {id} was not Found");
        }

    }
    public async Task<IResult> UpdateProductPrice(int id, decimal price)
    {
        var local = _context.Products.Local.FirstOrDefault(p => p.Id == id);

        if (local != null)
            _context.Entry(local).State = EntityState.Detached;

        var retrievedProduct = await _context.Products.AsNoTracking().SingleOrDefaultAsync(p => p.Id == id);
        if (retrievedProduct != null)
        {
            retrievedProduct.PriceAfterDiscount = price;
            try
            {
                _context.Update(retrievedProduct);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
            return Results.Ok(retrievedProduct);
        }
        else
        {
            return Results.NotFound($"Product with ProductID = {id} was not Found");
        }

    }
    public async Task<IResult> RemoveProduct(int productId)
    {
        var local = _context.Products.Local.FirstOrDefault(p => p.Id == productId);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        var product = await _context.Products.AsNoTracking().SingleOrDefaultAsync(p => p.Id == productId);
        if (product != null)
        {
            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return Results.Ok(product);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
            }

        }
        else
        {
            return Results.NotFound($"Product with ProductID = {productId} was not Found");
        }
    }
    #region Product Images Functions
    public async Task<IResult> GetProductImageDetails()
    {
        var productImageDetails = await _context.Productimages.AsNoTracking().ToListAsync();
        return productImageDetails == null || productImageDetails.Count == 0 ? Results.NotFound("No Product Image Details Found") : Results.Ok(productImageDetails);
    }
    public async Task<IResult> GetProductImageUrl(string productId)
    {
        var imageUrl = await _context.Productimages.AsNoTracking().SingleOrDefaultAsync(i => i.ProductId == productId);
        return imageUrl == null ? Results.NotFound($"No image with productId = {productId} was found") : Results.Ok(imageUrl);
    }
    public async Task<IResult> CreateProductImagesDetails(HttpRequest request)
    {
        var form = await request.ReadFormAsync();

        //if (!int.TryParse(form["ProductId"], out int productId))
        //{
        //    return Results.BadRequest("Invalid or missing ProductId.");
        //}
        var productId = form["ProductId"].ToString();
        if (string.IsNullOrEmpty(productId))
            return Results.BadRequest("Invalid or missing ProductId.");

        bool isPrimary = false;
        if (bool.TryParse(form["IsPrimary"], out bool parsedIsPrimary))
            isPrimary = parsedIsPrimary;


        var file = form.Files.Count > 0 ? form.Files[0] : null;
        if (file == null)
            return Results.BadRequest("No image file provided");
        var imageUrl = await GetProductImageUrl(file);

        var newProductImageDetails = new Productimage
        {
            ProductId = productId,
            ImageUrl = imageUrl,
            IsPrimary = isPrimary,
            CreatedAt = DateTime.Now
        };

        try
        {
            _context.Productimages.Add(newProductImageDetails);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }

        return Results.Ok(newProductImageDetails);
    }

    public async Task<IResult> DeleteProductImagesDetails(int id)
    {
        var local = _context.Productimages.Local.FirstOrDefault(pI => pI.ImageId == id);

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        var existingProductImage = await _context.Productimages.FindAsync(id);
        if (existingProductImage == null)
        {
            return Results.NotFound("Product image not found");
        }

        _context.Productimages.Remove(existingProductImage);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }

        return Results.Ok("Product image details deleted successfully");
    }


    #endregion

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
                InventoryId = worksheet.Cells[row, 1].Text,
                Name = worksheet.Cells[row, 2].Text,
                Price = decimal.Parse(worksheet.Cells[row, 3].Text),
                Category = int.Parse(worksheet.Cells[row, 4].Text),
                Subcategory = int.Parse(worksheet.Cells[row, 5].Text),
                Barcodes = worksheet.Cells[row, 6].Text,
                DiscountQuantity = int.Parse(worksheet.Cells[row, 7].Text),
                PurchaseCap = int.Parse(worksheet.Cells[row, 8].Text),
                TaxCode = worksheet.Cells[row, 9].Text,
                PriceAfterDiscount = decimal.Parse(worksheet.Cells[row, 10].Text)
            };
            products.Add(product);
        }
        return products;
    }
    public async Task<string> GetProductImageUrl(IFormFile formFile)
    {
        if (formFile == null)
            throw new ArgumentException("No file uploaded");
        var url = await _cloudinaryService.UploadImageAsync(formFile);
        return url;
    }

    #endregion
}
