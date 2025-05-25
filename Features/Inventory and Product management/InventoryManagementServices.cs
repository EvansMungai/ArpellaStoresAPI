using ArpellaStores.Services;

namespace ArpellaStores.Features.Inventory_and_Product_management;

public static class InventoryManagementServices
{
    public static void RegisterApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ICloudinaryService, CloudinaryService>();
        serviceCollection.AddTransient<ICategoriesService, CategoriesService>();
        serviceCollection.AddTransient<ISubcategoriesServices, SubcategoriesService>();
        serviceCollection.AddTransient<IInventoryService, InventoryService>();
        serviceCollection.AddTransient<IProductsService, ProductsService>();
        serviceCollection.AddTransient<ISupplierService, SupplierService>();
    }
}
