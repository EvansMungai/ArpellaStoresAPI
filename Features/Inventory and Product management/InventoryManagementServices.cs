using ArpellaStores.Features.Inventory_and_Product_management.Category_and_Subcategory_Managment.Services;
using ArpellaStores.Services;

namespace ArpellaStores.Features.Inventory_and_Product_management;

public static class InventoryManagementServices
{
    public static void RegisterApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ICloudinaryService, CloudinaryService>();
        serviceCollection.AddScoped<ICategoriesService, CategoriesService>();
        serviceCollection.AddScoped<ISubcategoriesServices, SubcategoriesService>();
        serviceCollection.AddScoped<IInventoryService, InventoryService>();
        serviceCollection.AddScoped<IProductsService, ProductsService>();
        serviceCollection.AddScoped<ISupplierService, SupplierService>();
    }
}
