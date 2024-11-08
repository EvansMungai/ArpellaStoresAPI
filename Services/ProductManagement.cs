using ArpellaStores.Data;
using ArpellaStores.Models;

namespace ArpellaStores.Services
{
    public class ProductManagement : IProductManagement
    {
        private readonly ArpellaContext _context;
        public ProductManagement(ArpellaContext context)
        {
            _context = context;
        }
        public List<Product> GetProducts()
        {
            var products = _context.Products.ToList();
            return products;
        }
        public List<Product> GetProduct(string productId)
        {
            return _context.Products.Where(p => p.Id == productId).ToList();
        }
        public Product CreateProduct(Product product)
        {
            var newProduct = new Product
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Category = product.Category
            };
            _context.Products.Add(newProduct);
            _context.SaveChangesAsync();
            return newProduct;
        }
        public Product? UpdateProductDetails(Product product, string id)
        {
            var retrievedProduct = _context.Products.FirstOrDefault(p => p.Id == id);
            if (retrievedProduct != null)
            {
                retrievedProduct.Id = product.Id;
                retrievedProduct.Name = product.Name;
                retrievedProduct.Price = product.Price;
                retrievedProduct.Category = product.Category;
                _context.Update(retrievedProduct);
                _context.SaveChanges();
            }
            return retrievedProduct;
        }
        public Product? RemoveProduct(string productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                _context.Remove(product);
                _context.SaveChanges();
            }
            return product;
        }
    }
}
