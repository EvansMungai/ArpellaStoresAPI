using ArpellaStores.Models;
using ArpellaStores.Services;

namespace ArpellaStores.Helpers
{
    public class RouteResolutionHelper : IRouteResolutionHelper
    {
        private readonly ICategoriesService _categoriesService;
        private readonly ISubcategoriesServices _subcategoriesServices;
        private readonly IProductManagement _productManagement;
        public RouteResolutionHelper(ICategoriesService categoriesService, ISubcategoriesServices subcategoriesServices, IProductManagement productManagement)
        {
            _categoriesService = categoriesService;
            _subcategoriesServices = subcategoriesServices;
            _productManagement = productManagement;
        }
        public void addMappings(WebApplication app)
        {
            app.MapGet("/", () => "Hello World!");
            // Products Routes
            //app.MapGet("/products", () => this._productManagement.GetProducts());
            //app.MapGet("/products/{id}", (string id) => this._productManagement.GetProduct(id));
            //app.MapPost("/products", (Product product) => this._productManagement.CreateProduct(product));
            //app.MapPut("/products/{id}", (Product product, string id) => this._productManagement.UpdateProductDetails(product, id));
            //app.MapDelete("/products/{id}", (string id) => this._productManagement.RemoveProduct(id));

            // Categories Routes
            app.MapGet("/categories", ()=> this._categoriesService.GetCategories()).WithTags("Categories").Produces(200).Produces(404).Produces<List<Category>>();
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
        }
    }
}
