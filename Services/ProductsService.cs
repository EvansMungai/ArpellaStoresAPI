using ArpellaStores.Data;
using ArpellaStores.Models;
using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;
using System.Globalization;

namespace ArpellaStores.Services
{
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
                Category = product.Category,
                Subcategory = product.Subcategory
            };
            try
            {
                _context.Products.Add(newProduct);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { }
            return Results.Ok(newProduct);
        }
        public async Task<IResult> CreateProducts(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0) return Results.BadRequest("File is empty");
                var products = file.FileName.EndsWith("csv") ? ParseCsv(file.OpenReadStream()) : ParseExcel(file.OpenReadStream());
                if (products == null || !products.Any())
                {
                    return Results.NotFound("No valid data found in the file");
                }

                _context.Products.AddRangeAsync(products);
                await _context.SaveChangesAsync();
                return Results.Ok(products);

            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.InnerException.Message);
            }
        }
        public async Task<IResult> UpdateProductDetails(Product product, string id)
        {
            var retrievedProduct = _context.Products.FirstOrDefault(p => p.Id == id);
            if (retrievedProduct != null)
            {
                retrievedProduct.Id = product.Id;
                retrievedProduct.Name = product.Name;
                retrievedProduct.Price = product.Price;
                retrievedProduct.Category = product.Category;
                retrievedProduct.Subcategory = product.Subcategory;
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
        #region Product Images Functions
        public async Task<IResult> GetProductImageDetails()
        {
            var productImageDetails = _context.Productimages.ToList();
            return productImageDetails == null || productImageDetails.Count == 0 ? Results.NotFound("No Product Image Details Found") : Results.Ok(productImageDetails);
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


            var file = form.Files.FirstOrDefault();
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
                return Results.BadRequest(ex.Message);
            }

            return Results.Ok(newProductImageDetails);
        }

        public async Task<IResult> DeleteProductImagesDetails(int id)
        {
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
                    Id = worksheet.Cells[row, 1].Text,
                    Name = worksheet.Cells[row, 2].Text,
                    Price = decimal.Parse(worksheet.Cells[row, 3].Text),
                    Category = worksheet.Cells[row, 4].Text,
                    Subcategory = worksheet.Cells[row, 5].Text,
                    Barcodes = worksheet.Cells[row, 6].Text,
                    TaxRate = decimal.Parse(worksheet.Cells[row, 7].Text),
                    DiscountQuantity = int.Parse(worksheet.Cells[row, 8].Text)
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
}
