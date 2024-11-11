using ArpellaStores.Data;
using ArpellaStores.Models;

namespace ArpellaStores.Services
{
    public class SubcategoriesService : ISubcategoriesServices
    {
        private readonly ArpellaContext _context;
        public SubcategoriesService(ArpellaContext context)
        {
            _context = context;
        }
        public async Task<List<Subcategory>> GetSubcategories()
        {
            var subcategories = _context.Subcategories.ToList();
            return subcategories;
        }
        public Subcategory CreateSubcategory(Subcategory subcategory)
        {
            var newSubcategory = new Subcategory
            {
                Id = subcategory.Id,
                SubcategoryName = subcategory.SubcategoryName,
                CategoryId = subcategory.CategoryId
            };
            _context.Add(newSubcategory);
            _context.SaveChangesAsync();
            return newSubcategory;
        }
        public Subcategory? RemoveSubcategory(string id)
        {
            var retrievedSubCategory = _context.Subcategories.FirstOrDefault(c => c.Id == id);
            if (retrievedSubCategory != null)
            {
                _context.Remove(retrievedSubCategory);
                _context.SaveChangesAsync();
            };
            return retrievedSubCategory;
        }
    }
}
