using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class ProductsService : IProductsService
{
    private readonly IProductRepository _repo;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IProductHelper _helper;
    public ProductsService(IProductRepository repo, ICloudinaryService cloudinaryService, IProductHelper helper)
    {
        _repo = repo;
        _cloudinaryService = cloudinaryService;
        _helper = helper;
    }
    public async Task<IResult> GetProducts()
    {
        var products = await _repo.GetAllProductsAsync();
        return products == null || products.Count == 0 ? Results.NotFound("No Products Found") : Results.Ok(products);
    }
    public async Task<IResult> GetProduct(int productId)
    {
        var product = await _repo.GetProductByIdAsync(productId);
        return product == null ? Results.NotFound($"Product with ProductID = {productId} was not found") : Results.Ok(product);
    }
    public async Task<IResult> CreateProduct(Product product)
    {

        var existing = await _repo.GetProductByIdAsync(product.Id);
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
            PurchaseCap = product.PurchaseCap
        };
        try
        {
            await _repo.AddProductAsync(newProduct);
            return Results.Ok(newProduct);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
    }
    public async Task<IResult> CreateProducts(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return Results.BadRequest("File is empty");

            using var stream = file.OpenReadStream();
            var (products, validationErrors) = _helper.ParseExcel(stream);

            var result = new ProductUploadResult
            {
                ValidProducts = products,
                ValidationErrors = validationErrors
            };

            if (products.Any())
                await _repo.AddProductsAsync(products);

            return Results.Ok(new
            {
                message = products.Any()
                    ? "Upload completed with no errors."
                    : "Upload failed. No valid records were inserted.",
                insertedRecords = result.InsertedCount,
                errors = result.ValidationErrors
            });
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
    }


    public async Task<IResult> UpdateProductDetails(Product product, int id)
    {
        var retrievedProduct = await _repo.GetProductByIdAsync(id);
        if (retrievedProduct == null) return Results.NotFound($"Product with ProductID = {id} was not Found");

        try
        {
            await _repo.UpdateProductDetails(product, id);
            return Results.Ok(product);
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
    }
    public async Task<IResult> UpdateProductPrice(int id, decimal price)
    {
        var retrievedProduct = await _repo.GetProductByIdAsync(id);
        if (retrievedProduct == null)
            return Results.NotFound($"Product with ProductID = {id} was not Found");

        try
        {
            await _repo.UpdateProductPrice(id, price);
            return Results.Ok(retrievedProduct);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
    }
    public async Task<IResult> RemoveProduct(int productId)
    {
        var product = await _repo.GetProductByIdAsync(productId);
        if (product == null)
            return Results.NotFound($"Product with ProductID = {productId} was not Found");

        try
        {
            await _repo.RemoveProduct(productId);
            return Results.Ok(product);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
    }
}
