namespace ArpellaStores.Features.InventoryManagement.Services;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile file);
}
