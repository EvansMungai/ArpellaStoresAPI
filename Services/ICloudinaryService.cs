namespace ArpellaStores.Services;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile file);
}
