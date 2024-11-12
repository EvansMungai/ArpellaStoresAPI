using ArpellaStores.Models;
using ArpellaStores.Services;

namespace ArpellaStores.Helpers
{
    public class RouteResolutionHelper : IRouteResolutionHelper
    {
        private readonly ICategoriesService _categoriesService;
        private readonly ISubcategoriesServices _subcategoriesServices;
        private readonly IProductsService _productsService;
        public RouteResolutionHelper(ICategoriesService categoriesService, ISubcategoriesServices subcategoriesServices, IProductsService productsService)
        {
            _categoriesService = categoriesService;
            _subcategoriesServices = subcategoriesServices;
            _productsService = productsService;
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
        }
    }
}
