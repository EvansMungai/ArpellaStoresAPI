namespace ArpellaStores.Features.InventoryManagement.Services;

public static class ServiceRegistration
{
    public static void RegisterApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ICloudinaryService, CloudinaryService>();
        serviceCollection.AddScoped<ICategoriesService, CategoriesService>();
        serviceCollection.AddScoped<ICategoryRepository, CategoryRepository>();
        serviceCollection.AddScoped<ISubcategoriesServices, SubcategoriesService>();
        serviceCollection.AddScoped<ISubcategoryRepository, SubcategoryRepository>();
        serviceCollection.AddScoped<IInventoryService, InventoryService>();
        serviceCollection.AddScoped<IInvoiceService, InvoiceService>();
        serviceCollection.AddScoped<IProductsService, ProductsService>();
        serviceCollection.AddScoped<ISupplierService, SupplierService>();
    }
}
