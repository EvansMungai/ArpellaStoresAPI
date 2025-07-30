
using ArpellaStores.Features.InventoryManagement.Models;

namespace ArpellaStores.Features.InventoryManagement.Services;

public class ProductImageService : IProductImageService
{
    private readonly IProductImageRepository _repo;
    private readonly ICloudinaryService _cloudinaryService;
    public ProductImageService(IProductImageRepository repo, ICloudinaryService cloudinaryService)
    {
        _repo = repo;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<IResult> CreateProductImagesDetails(HttpRequest request)
    {
        var form = await request.ReadFormAsync();

        if (!int.TryParse(form["ProductId"], out int productId))
        {
            return Results.BadRequest("Invalid or missing ProductId.");
        }
        //var productId = form["ProductId"].ToString();
        //if (string.IsNullOrEmpty(productId))
        //    return Results.BadRequest("Invalid or missing ProductId.");

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
            await _repo.AddProductImageDetails(newProductImageDetails);
            return Results.Ok(newProductImageDetails);
        }
        catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message ?? ex.Message); }
    }

    public async Task<IResult> DeleteProductImagesDetails(int id)
    {
        var existingProductImage = await _repo.GetProductImageObjectById(id);
        if (existingProductImage == null)
            return Results.NotFound("Product image not found");

        try
        {
            await _repo.RemoveProductImage(id);
            return Results.Ok("Product image details deleted successfully");
        }
        catch (Exception ex) { return Results.BadRequest(ex.Message); }

    }

    public async Task<IResult> GetProductImageDetails()
    {
        var productImageDetails = await _repo.GetProductImageDetailsAsync();
        return productImageDetails == null || productImageDetails.Count == 0 ? Results.NotFound("No Product Image Details Found") : Results.Ok(productImageDetails);
    }

    public async Task<IResult> GetProductImageUrl(int productId)
    {
        var imageUrl = await _repo.GetProductImageUrlAsync(productId);
        return imageUrl == null ? Results.NotFound($"No image with productId = {productId} was found") : Results.Ok(imageUrl);
    }
    #region Utilities
    public async Task<string> GetProductImageUrl(IFormFile formFile)
    {
        if (formFile == null)
            throw new ArgumentException("No file uploaded");
        var url = await _cloudinaryService.UploadImageAsync(formFile);
        return url;
    }
    #endregion
}
