using ArpellaStores.Models;
using ArpellaStores.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;

namespace ArpellaStores.Helpers;

public class RouteResolutionHelper : IRouteResolutionHelper
{
    private readonly ICategoriesService _categoriesService;
    private readonly ISubcategoriesServices _subcategoriesServices;
    private readonly IProductsService _productsService;
    private readonly IInventoryService _inventoryService;
    private readonly IDiscountService _discountService;
    private readonly ICouponService _couponService;
    private readonly IFlashsaleService _flashsaleService;
    private readonly IOrderService _orderService;
    private readonly IUserManagementService _userManagementService;
    private readonly IAuthenticationService _authenticationService;
    public RouteResolutionHelper(ICategoriesService categoriesService, ISubcategoriesServices subcategoriesServices, IProductsService productsService, IInventoryService inventoryService, IDiscountService discountService, ICouponService couponService, IFlashsaleService flashsaleService, IOrderService orderService ,IUserManagementService userManagementService, IAuthenticationService authenticationService)
    {
        _categoriesService = categoriesService;
        _subcategoriesServices = subcategoriesServices;
        _productsService = productsService;
        _inventoryService = inventoryService;
        _discountService = discountService;
        _couponService = couponService;
        _flashsaleService = flashsaleService;
        _orderService = orderService;
        _userManagementService = userManagementService;
        _authenticationService = authenticationService;
        
    }
    public void addMappings(WebApplication app)
    {
        app.MapGet("/", () => "Welcome to Arpella Stores Web API!");

        // Authentication Routes
        app.MapPost("/register", (UserManager<User> userManager, User model) => this._authenticationService.RegisterUser(userManager, model)).WithTags("Authentication");
        app.MapPost("/login", (SignInManager<User> signInManager, UserManager<User> userManager, User model) => this._authenticationService.Login(signInManager, userManager, model)).WithTags("Authentication");
        app.MapPost("/logout", (SignInManager<User> signInManager) => this._authenticationService.LogOut(signInManager)).WithTags("Authentication");

        #region Admin Routes
        // Roles
        app.MapGet("/roles", ()=> this._userManagementService.GetRoles()).WithTags("Admin").Produces(200).Produces(404);
        app.MapGet("/role/{id}", (string role) => this._userManagementService.EnsureRoleExists(role)).WithTags("Admin").Produces(200).Produces(404);
        app.MapPost("/role", (string role) => this._userManagementService.CreateRole(role)).WithTags("Admin").Produces(200).Produces(404);
        app.MapPut("/role/{id}", (string roleId, string roleName) => this._userManagementService.EditRole(roleId, roleName)).WithTags("Admin").Produces(200).Produces(404);
        app.MapDelete("/role/{role}", (string role) => this._userManagementService.RemoveRole(role)).WithTags("Admin").Produces(200).Produces(404);
        // Users
        app.MapGet("/users", ()=> this._userManagementService.GetUsers()).WithTags("Admin").Produces(200).Produces(404).Produces<List<User>>();
        app.MapGet("/user/{id}", (string id)=> this._userManagementService.GetUser(id)).WithTags("Admin").Produces(200).Produces(404).Produces<User>();
        app.MapGet("/special-users", ()=> this._userManagementService.GetSpecialUsers()).WithTags("Admin").Produces(200).Produces(404).Produces<User>();
        app.MapDelete("/user/{id}", (string id) => this._userManagementService.RemoveUser(id)).WithTags("Admin").Produces(200).Produces(404).Produces<User>();
        app.MapPut("/userrole/{id}", (string id, string role) => this._userManagementService.AssignRoleToUserAsync(id, role)).WithTags("Admin").Produces(200).Produces(404).Produces<User>();
        app.MapPut("/userdetails/{id}", (string id, User update) => this._userManagementService.UpdateUserDetails(id, update)).WithTags("Admin").Produces(200).Produces(404).Produces<User>();
        app.MapPost("/control",  (UserManager<User> userManager, UserProfile profile) => this._userManagementService.RegisterSpecialUsers(userManager, profile.User, profile.Role)).WithTags("Admin").Produces(200).Produces(404).Produces<User>();
        #endregion

        #region Inventory Manager Routes
        // Categories Routes
        app.MapGet("/categories", () => this._categoriesService.GetCategories()).WithTags("Categories").Produces(200).Produces(404).Produces<List<Category>>();
        app.MapGet("/category/{id}", (int id) => this._categoriesService.GetCategory(id)).WithTags("Categories").Produces(200).Produces(404).Produces<Category>();
        app.MapPost("/category", (Category category) => this._categoriesService.CreateCategory(category)).WithTags("Categories").Produces<Category>();
        app.MapPut("/category/{id}", (Category category, int id) => this._categoriesService.UpdateCategoryDetails(category, id)).WithTags("Categories").Produces<Category>();
        app.MapDelete("/category/{id}", (int id) => this._categoriesService.RemoveCategory(id)).WithTags("Categories").Produces(200).Produces(404).Produces<Category>();

        // Subcategories Routes
        app.MapGet("/subcategories", () => this._subcategoriesServices.GetSubcategories()).WithTags("Subcategories").Produces(200).Produces(404).Produces<List<Subcategory>>();
        app.MapGet("/subcategory/{id}", (int id) => this._subcategoriesServices.GetSubcategory(id)).WithTags("Subcategories").Produces(200).Produces(404).Produces<Subcategory>();
        app.MapPost("/subcategory", (Subcategory subcategory) => this._subcategoriesServices.CreateSubcategory(subcategory)).WithTags("Subcategories").Produces(200).Produces(404).Produces<Subcategory>();
        app.MapPut("/subcategory/{id}", (Subcategory subcategory, int id) => this._subcategoriesServices.UpdateSubcategoryDetails(subcategory, id)).WithTags("Subcategories").Produces<Subcategory>();
        app.MapDelete("/subcategory/{id}", (int id) => this._subcategoriesServices.RemoveSubcategory(id)).WithTags("Subcategories").Produces(200).Produces(404).Produces<Subcategory>();

        // Products Routes
        app.MapGet("/products", () => this._productsService.GetProducts()).WithTags("Products").Produces(200).Produces(404).Produces<List<Product>>();
        app.MapGet("/product/{id}", (int id) => this._productsService.GetProduct(id)).WithTags("Products").Produces(200).Produces(404).Produces<Product>();
        app.MapPost("/product", (Product product) => this._productsService.CreateProduct(product)).WithTags("Products").Produces(200).Produces(404).Produces<Product>();
        app.MapPost("/products", (IFormFile file) => this._productsService.CreateProducts(file)).WithTags("Products").Produces(200).Produces(404).Produces<Product>().DisableAntiforgery();
        app.MapPut("/product/{id}", (Product product, int id) => this._productsService.UpdateProductDetails(product, id)).WithTags("Products").Produces(200).Produces(404).Produces<Product>();
        app.MapDelete("/product/{id}", (int id) => this._productsService.RemoveProduct(id)).WithTags("Products").Produces(200).Produces(404).Produces<Product>();

        //Product Images Routes
        app.MapGet("/product-image-details", () => this._productsService.GetProductImageDetails()).WithTags("Product Images");
        app.MapPost("/product-image-details", (HttpRequest request) => this._productsService.CreateProductImagesDetails(request)).WithTags("Product Images");
        app.MapDelete("/product-image-details/{id}", (int id) => this._productsService.DeleteProductImagesDetails(id)).WithTags("Product Images").Produces(200).Produces(404);

        // Inventory Routes
        app.MapGet("/inventories", () => this._inventoryService.GetInventories()).WithTags("Inventories").Produces(200).Produces(404).Produces<List<Inventory>>();
        app.MapGet("/inventory/{id}", (int id) => this._inventoryService.GetInventory(id)).WithTags("Inventories").Produces(200).Produces(404).Produces<Inventory>();
        app.MapPost("/inventory", (Inventory inventory) => this._inventoryService.CreateInventory(inventory)).WithTags("Inventories").Produces(200).Produces(404).Produces<Inventory>();
        app.MapPost("/inventories", (IFormFile file) => this._inventoryService.CreateInventories(file)).WithTags("Inventories").Produces(200).Produces(404).Produces<List<Inventory>>().DisableAntiforgery();
        app.MapPut("/inventory/{id}", (Inventory inventory, int id) => this._inventoryService.UpdateInventory(inventory, id)).WithTags("Inventories").Produces(200).Produces(404).Produces<Inventory>();
        app.MapDelete("/inventory/{id}", (int id) => this._inventoryService.RemoveInventory(id)).WithTags("Inventories").Produces(200).Produces(404).Produces<Inventory>();
        app.MapGet("/inventorylevels", () => this._inventoryService.CheckInventoryLevels()).WithTags("Inventories").Produces(200).Produces(404).Produces<List<Inventory>>();

        // Final Price Route
        app.MapGet("/final-price", (int productId, string? couponCode) => this._discountService.GetFinalPrice(productId, couponCode)).WithTags("Final Price").Produces(200).Produces(404);

        // Coupons Route
        app.MapGet("/coupons", () => this._couponService.GetCoupons()).WithTags("Coupons").Produces(200).Produces(404).Produces<List<Coupon>>();
        app.MapGet("/coupon/{id}", (int id) => this._couponService.GetCoupon(id)).WithTags("Coupons").Produces(200).Produces(404).Produces<Coupon>();
        app.MapPost("/coupon", (Coupon coupon) => this._couponService.CreateCoupon(coupon)).WithTags("Coupons").Produces(200).Produces(500).Produces<Coupon>();
        app.MapPut("/coupon/{id}", (Coupon coupon, int id) => this._couponService.UpdateCoupon(coupon, id)).WithTags("Coupons").Produces(200).Produces(404).Produces(500).Produces<Coupon>();
        app.MapDelete("/coupon/{id}", (int id) => this._couponService.RemoveCoupon(id)).WithTags("Coupons").Produces(200).Produces(404).Produces(500).Produces<Coupon>();

        // Flashsale Route
        app.MapGet("/flashsales", () => this._flashsaleService.GetFlashSales()).WithTags("Flash Sales").Produces(200).Produces(404).Produces<List<Flashsale>>();
        app.MapGet("/flashsale/{id}", (int id) => this._flashsaleService.GetFlashSale(id)).WithTags("Flash Sales").Produces(200).Produces(404).Produces<Flashsale>();
        app.MapPost("/flashsale", (Flashsale flashsale) => this._flashsaleService.CreateFlashSale(flashsale)).WithTags("Flash Sales").Produces(200).Produces(500).Produces<Flashsale>();
        app.MapPut("/flashsale/{id}", (Flashsale flashsale, int id) => this._flashsaleService.UpdateFlashSale(flashsale, id)).WithTags("Flash Sales").Produces(200).Produces(404).Produces(500).Produces<Flashsale>();
        app.MapDelete("/flashsale/{id}", (int id) => this._flashsaleService.RemoveFlashsale(id)).WithTags("Flash Sales").Produces(200).Produces(404).Produces(500).Produces<Flashsale>();
        #endregion

        #region Orders Routes
        app.MapGet("/orders", ()=>  this._orderService.GetOrders()).WithTags("Orders").Produces(200).Produces(404).Produces<List<Order>>();
        app.MapGet("/order/{id}", (string id) => this._orderService.GetOrder(id)).WithTags("Orders").Produces(200).Produces(404).Produces<Order>();
        app.MapPost("/order", (Order order) => this._orderService.CreateOrder(order)).WithTags("Orders").Produces(200).Produces(404).Produces<Order>();
        app.MapDelete("/order/{id}", (string id) => this._orderService.RemoveOrder(id)).WithTags("Orders").Produces(200).Produces(404).Produces<Order>();
        #endregion
    }
}
