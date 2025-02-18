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
    private readonly IUserManagementService _userManagementService;
    private readonly IAuthenticationService _authenticationService;
    public RouteResolutionHelper(ICategoriesService categoriesService, ISubcategoriesServices subcategoriesServices, IProductsService productsService, IInventoryService inventoryService, IDiscountService discountService, ICouponService couponService, IFlashsaleService flashsaleService,IUserManagementService userManagementService, IAuthenticationService authenticationService)
    {
        _categoriesService = categoriesService;
        _subcategoriesServices = subcategoriesServices;
        _productsService = productsService;
        _inventoryService = inventoryService;
        _discountService = discountService;
        _couponService = couponService;
        _flashsaleService = flashsaleService;
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
        app.MapGet("/roles", ()=> this._userManagementService.GetRoles()).WithTags("Admin").Produces(200).Produces(404).RequireAuthorization("AdminPolicy");
        app.MapGet("/role/{id}", (string role) => this._userManagementService.EnsureRoleExists(role)).WithTags("Admin").Produces(200).Produces(404).RequireAuthorization("AdminPolicy");
        app.MapPost("/role", (string role) => this._userManagementService.CreateRole(role)).WithTags("Admin").Produces(200).Produces(404).RequireAuthorization("AdminPolicy");
        app.MapPut("/role/{id}", (string roleId, string roleName) => this._userManagementService.EditRole(roleId, roleName)).WithTags("Admin").Produces(200).Produces(404).RequireAuthorization("AdminPolicy");
        app.MapDelete("/role/{role}", (string role) => this._userManagementService.RemoveRole(role)).WithTags("Admin").Produces(200).Produces(404).RequireAuthorization("AdminPolicy");
        // Users
        app.MapGet("/users", ()=> this._userManagementService.GetUsers()).WithTags("Admin").Produces(200).Produces(404).Produces<List<User>>().RequireAuthorization("AdminPolicy");
        app.MapGet("/userdetails/{number}", (string number)=> this._userManagementService.GetUserDetails(number)).WithTags("Admin").Produces(200).Produces(404).Produces<List<User>>().RequireAuthorization("AdminPolicy");
        app.MapGet("/user/{id}", (string id)=> this._userManagementService.GetUser(id)).WithTags("Admin").Produces(200).Produces(404).Produces<User>().RequireAuthorization("AdminPolicy");
        app.MapGet("/special-users", ()=> this._userManagementService.GetSpecialUsers()).WithTags("Admin").Produces(200).Produces(404).Produces<User>().RequireAuthorization("AdminPolicy");
        app.MapDelete("/user/{id}", (string id) => this._userManagementService.RemoveUser(id)).WithTags("Admin").Produces(200).Produces(404).Produces<User>().RequireAuthorization("AdminPolicy");
        app.MapPut("/userrole/{id}", (string id, string role) => this._userManagementService.AssignRoleToUserAsync(id, role)).WithTags("Admin").Produces(200).Produces(404).Produces<User>().RequireAuthorization();
        app.MapPost("/control",  (UserManager<User> userManager, UserProfile profile) => this._userManagementService.RegisterSpecialUsers(userManager, profile.User, profile.Role)).WithTags("Admin").Produces(200).Produces(404).Produces<User>();
        #endregion

        #region Inventory Manager Routes
        // Categories Routes
        app.MapGet("/categories", () => this._categoriesService.GetCategories()).WithTags("Categories").Produces(200).Produces(404).Produces<List<Category>>().RequireAuthorization("InventoryManagerPolicy");
        app.MapGet("/categories/{id}", (string id) => this._categoriesService.GetCategory(id)).WithTags("Categories").Produces(200).Produces(404).Produces<Category>().RequireAuthorization("InventoryManagerPolicy");
        app.MapPost("/categories", (Category category) => this._categoriesService.CreateCategory(category)).WithTags("Categories").Produces<Category>().RequireAuthorization("InventoryManagerPolicy");
        app.MapPut("/categories/{id}", (Category category, string id) => this._categoriesService.UpdateCategoryDetails(category, id)).WithTags("Categories").Produces<Category>().RequireAuthorization("InventoryManagerPolicy");
        app.MapDelete("/categories/{id}", (string id) => this._categoriesService.RemoveCategory(id)).WithTags("Categories").Produces(200).Produces(404).Produces<Category>().RequireAuthorization("InventoryManagerPolicy");

        // Subcategories Routes
        app.MapGet("/subcategories", () => this._subcategoriesServices.GetSubcategories()).WithTags("Subcategories").Produces(200).Produces(404).Produces<List<Subcategory>>().RequireAuthorization("InventoryManagerPolicy");
        app.MapGet("/subcategories/{id}", (string id) => this._subcategoriesServices.GetSubcategory(id)).WithTags("Subcategories").Produces(200).Produces(404).Produces<Subcategory>().RequireAuthorization("InventoryManagerPolicy");
        app.MapPost("/subcategories", (Subcategory subcategory) => this._subcategoriesServices.CreateSubcategory(subcategory)).WithTags("Subcategories").Produces(200).Produces(404).Produces<Subcategory>().RequireAuthorization("InventoryManagerPolicy");
        app.MapPut("/subcategories/{id}", (Subcategory subcategory, string id) => this._subcategoriesServices.UpdateSubcategoryDetails(subcategory, id)).WithTags("Subcategories").Produces<Subcategory>().RequireAuthorization("InventoryManagerPolicy");
        app.MapDelete("/subcategories/{id}", (string id) => this._subcategoriesServices.RemoveSubcategory(id)).WithTags("Subcategories").Produces(200).Produces(404).Produces<Subcategory>().RequireAuthorization("InventoryManagerPolicy");

        // Products Routes
        app.MapGet("/products", () => this._productsService.GetProducts()).WithTags("Products").Produces(200).Produces(404).Produces<List<Product>>().RequireAuthorization("InventoryManagerPolicy");
        app.MapGet("/products/{id}", (string id) => this._productsService.GetProduct(id)).WithTags("Products").Produces(200).Produces(404).Produces<Product>().RequireAuthorization("InventoryManagerPolicy");
        app.MapPost("/products", (Product product) => this._productsService.CreateProduct(product)).WithTags("Products").Produces(200).Produces(404).Produces<Product>().RequireAuthorization("InventoryManagerPolicy");
        app.MapPut("/products/{id}", (Product product, string id) => this._productsService.UpdateProductDetails(product, id)).WithTags("Products").Produces(200).Produces(404).Produces<Product>().RequireAuthorization("InventoryManagerPolicy");
        app.MapDelete("/products/{id}", (string id) => this._productsService.RemoveProduct(id)).WithTags("Products").Produces(200).Produces(404).Produces<Product>().RequireAuthorization("InventoryManagerPolicy");

        // Inventory Routes
        app.MapGet("/inventories", () => this._inventoryService.GetInventories()).WithTags("Inventories").Produces(200).Produces(404).Produces<List<Inventory>>().RequireAuthorization("InventoryManagerPolicy");
        app.MapGet("/inventory/{id}", (int id) => this._inventoryService.GetInventory(id)).WithTags("Inventories").Produces(200).Produces(404).Produces<Inventory>().RequireAuthorization("InventoryManagerPolicy");
        app.MapPost("/inventories", (Inventory inventory) => this._inventoryService.CreateInventory(inventory)).WithTags("Inventories").Produces(200).Produces(404).Produces<Inventory>().RequireAuthorization("InventoryManagerPolicy");
        app.MapPut("/inventory/{id}", (Inventory inventory, int id) => this._inventoryService.UpdateInventory(inventory, id)).WithTags("Inventories").Produces(200).Produces(404).Produces<Inventory>().RequireAuthorization("InventoryManagerPolicy");
        app.MapDelete("/inventory/{id}", (int id) => this._inventoryService.RemoveInventory(id)).WithTags("Inventories").Produces(200).Produces(404).Produces<Inventory>().RequireAuthorization("InventoryManagerPolicy");

        // Final Price Route
        app.MapGet("/final-price", (string productId, string? couponCode) => this._discountService.GetFinalPrice(productId, couponCode)).WithTags("Final Price").Produces(200).Produces(404).RequireAuthorization("InventoryManagerPolicy");

        // Coupons Route
        app.MapGet("/coupons", () => this._couponService.GetCoupons()).WithTags("Coupons").Produces(200).Produces(404).Produces<List<Coupon>>().RequireAuthorization("InventoryManagerPolicy");
        app.MapGet("/coupon/{id}", (int id) => this._couponService.GetCoupon(id)).WithTags("Coupons").Produces(200).Produces(404).Produces<Coupon>().RequireAuthorization("InventoryManagerPolicy");
        app.MapPost("/coupon", (Coupon coupon) => this._couponService.CreateCoupon(coupon)).WithTags("Coupons").Produces(200).Produces(500).Produces<Coupon>().RequireAuthorization("InventoryManagerPolicy");
        app.MapPut("/coupon/{id}", (Coupon coupon, int id) => this._couponService.UpdateCoupon(coupon, id)).WithTags("Coupons").Produces(200).Produces(404).Produces(500).Produces<Coupon>().RequireAuthorization("InventoryManagerPolicy");
        app.MapDelete("/coupon/{id}", (int id) => this._couponService.RemoveCoupon(id)).WithTags("Coupons").Produces(200).Produces(404).Produces(500).Produces<Coupon>().RequireAuthorization("InventoryManagerPolicy");

        // Flashsale Route
        app.MapGet("/flashsales", () => this._flashsaleService.GetFlashSales()).WithTags("Flash Sales").Produces(200).Produces(404).Produces<List<Flashsale>>().RequireAuthorization("InventoryManagerPolicy");
        app.MapGet("/flashsale/{id}", (int id) => this._flashsaleService.GetFlashSale(id)).WithTags("Flash Sales").Produces(200).Produces(404).Produces<Flashsale>().RequireAuthorization("InventoryManagerPolicy");
        app.MapPost("/flashsale", (Flashsale flashsale) => this._flashsaleService.CreateFlashSale(flashsale)).WithTags("Flash Sales").Produces(200).Produces(500).Produces<Flashsale>().RequireAuthorization("InventoryManagerPolicy");
        app.MapPut("/flashsale/{id}", (Flashsale flashsale, int id) => this._flashsaleService.UpdateFlashSale(flashsale, id)).WithTags("Flash Sales").Produces(200).Produces(404).Produces(500).Produces<Flashsale>().RequireAuthorization("InventoryManagerPolicy");
        app.MapDelete("/flashsale/{id}", (int id) => this._flashsaleService.RemoveFlashsale(id)).WithTags("Flash Sales").Produces(200).Produces(404).Produces(500).Produces<Flashsale>().RequireAuthorization("InventoryManagerPolicy");
        #endregion


    }
}
