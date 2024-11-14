using ArpellaStores.Models;
using ArpellaStores.Services;

namespace ArpellaStores.Helpers;

public class RouteResolutionHelper : IRouteResolutionHelper
{
    private readonly ICategoriesService _categoriesService;
    private readonly ISubcategoriesServices _subcategoriesServices;
    private readonly IProductsService _productsService;
    private readonly IInventoryService _inventoryService;
    private readonly IDiscountService _discountService;
    private readonly ICouponService _couponService;
    public RouteResolutionHelper(ICategoriesService categoriesService, ISubcategoriesServices subcategoriesServices, IProductsService productsService, IInventoryService inventoryService, IDiscountService discountService, ICouponService couponService)
    {
        _categoriesService = categoriesService;
        _subcategoriesServices = subcategoriesServices;
        _productsService = productsService;
        _inventoryService = inventoryService;
        _discountService = discountService;
        _couponService = couponService;
    }
    public void addMappings(WebApplication app)
    {
        app.MapGet("/", () => "Hello World!");

        // Categories Routes
        app.MapGet("/categories", () => this._categoriesService.GetCategories()).WithTags("Categories").Produces(200).Produces(404).Produces<List<Category>>();
        app.MapGet("/categories/{id}", (string id) => this._categoriesService.GetCategory(id)).WithTags("Categories").Produces(200).Produces(404).Produces<Category>();
        app.MapPost("/categories", (Category category) => this._categoriesService.CreateCategory(category)).WithTags("Categories").Produces<Category>();
        app.MapPut("/categories/{id}", (Category category, string id) => this._categoriesService.UpdateCategoryDetails(category, id)).WithTags("Categories").Produces<Category>();
        app.MapDelete("/categories/{id}", (string id) => this._categoriesService.RemoveCategory(id)).WithTags("Categories").Produces(200).Produces(404).Produces<Category>();

        // Subcategories Routes
        app.MapGet("/subcategories", () => this._subcategoriesServices.GetSubcategories()).WithTags("Subcategories").Produces(200).Produces(404).Produces<List<Subcategory>>();
        app.MapGet("/subcategories/{id}", (string id) => this._subcategoriesServices.GetSubcategory(id)).WithTags("Subcategories").Produces(200).Produces(404).Produces<Subcategory>();
        app.MapPost("/subcategories", (Subcategory subcategory) => this._subcategoriesServices.CreateSubcategory(subcategory)).WithTags("Subcategories").Produces(200).Produces(404).Produces<Subcategory>();
        app.MapPut("/subcategories/{id}", (Subcategory subcategory, string id) => this._subcategoriesServices.UpdateSubcategoryDetails(subcategory, id)).WithTags("Subcategories").Produces<Subcategory>();
        app.MapDelete("/subcategories/{id}", (string id) => this._subcategoriesServices.RemoveSubcategory(id)).WithTags("Subcategories").Produces(200).Produces(404).Produces<Subcategory>();

        // Products Routes
        app.MapGet("/products", () => this._productsService.GetProducts()).WithTags("Products").Produces(200).Produces(404).Produces<List<Product>>();
        app.MapGet("/products/{id}", (string id) => this._productsService.GetProduct(id)).WithTags("Products").Produces(200).Produces(404).Produces<Product>();
        app.MapPost("/products", (Product product) => this._productsService.CreateProduct(product)).WithTags("Products").Produces(200).Produces(404).Produces<Product>();
        app.MapPut("/products/{id}", (Product product, string id) => this._productsService.UpdateProductDetails(product, id)).WithTags("Products").Produces(200).Produces(404).Produces<Product>();
        app.MapDelete("/products/{id}", (string id) => this._productsService.RemoveProduct(id)).WithTags("Products").Produces(200).Produces(404).Produces<Product>();

        // Inventory Routes
        app.MapGet("/inventories", () => this._inventoryService.GetInventories()).WithTags("Inventories").Produces(200).Produces(404).Produces<List<Inventory>>();
        app.MapGet("/inventory/{id}", (int id) => this._inventoryService.GetInventory(id)).WithTags("Inventories").Produces(200).Produces(404).Produces<Inventory>();
        app.MapPost("/inventories", (Inventory inventory) => this._inventoryService.CreateInventory(inventory)).WithTags("Inventories").Produces(200).Produces(404).Produces<Inventory>();
        app.MapPut("/inventory/{id}", (Inventory inventory, int id) => this._inventoryService.UpdateInventory(inventory, id)).WithTags("Inventories").Produces(200).Produces(404).Produces<Inventory>();
        app.MapDelete("/inventory/{id}", (int id) => this._inventoryService.RemoveInventory(id)).WithTags("Inventories").Produces(200).Produces(404).Produces<Inventory>();

        // Final Price Route
        app.MapGet("/final-price", (string productId, string? couponCode) => this._discountService.GetFinalPrice(productId, couponCode)).WithTags("Final Price").Produces(200).Produces(404);

        // Coupons Route
        app.MapGet("/coupons", () => this._couponService.GetCoupons()).WithTags("Coupons").Produces(200).Produces(404).Produces<List<Coupon>>();
        app.MapGet("/coupon/{id}", (int id) => this._couponService.GetCoupon(id)).WithTags("Coupons").Produces(200).Produces(404).Produces<Coupon>();
        app.MapPost("/coupon", (Coupon coupon) => this._couponService.CreateCoupon(coupon)).WithTags("Coupons").Produces(200).Produces(500).Produces<Coupon>();
        app.MapPut("/coupon/{id}", (Coupon coupon, int id) => this._couponService.UpdateCoupon(coupon, id)).WithTags("Coupons").Produces(200).Produces(404).Produces(500).Produces<Coupon>();
        app.MapDelete("/coupon/{id}", (int id) => this._couponService.RemoveCoupon(id)).WithTags("Coupons").Produces(200).Produces(404).Produces(500).Produces<Coupon>();
    }
}
