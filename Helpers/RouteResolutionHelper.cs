using ArpellaStores.Services;
using ArpellaStores.Models;

namespace ArpellaStores.Helpers
{
    public class RouteResolutionHelper : IRouteResolutionHelper
    {
        private readonly IProductManagement _productManagement;
        public RouteResolutionHelper(IProductManagement productManagement)
        {
            _productManagement = productManagement;
        }
        public void addMappings(WebApplication app)
        {
            app.MapGet("/", () => "Hello World!");
            app.MapGet("/products", () => this._productManagement.GetProducts());
            app.MapGet("/products/{id}", (string id) => this._productManagement.GetProduct(id));
            app.MapPost("/products", (Product product) => this._productManagement.CreateProduct(product));
            app.MapPut("/products/{id}", (Product product, string id) => this._productManagement.UpdateProductDetails(product, id));
            app.MapDelete("/products/{id}", (string id) => this._productManagement.RemoveProduct(id));
        }
    }
}
