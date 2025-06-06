using ArpellaStores.Data.Infrastructure;
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
        public async Task<IResult> GetSubcategories()
        {
            var subcategories = _context.Subcategories.Select(s => new {s.Id, s.SubcategoryName, s.CategoryId}).ToList();
            return subcategories == null || subcategories.Count == 0 ? Results.NotFound("No Subcategories Found") : Results.Ok(subcategories);
        }
        public async Task<IResult> GetSubcategory(int id)
        {
            var retrievedSubcategory = _context.Subcategories.Select(s => new {s.Id, s.SubcategoryName, s.CategoryId}).FirstOrDefault(s => s.Id == id);
            return retrievedSubcategory == null ? Results.NotFound($"Subcategory of ID = {id} was not found") : Results.Ok(retrievedSubcategory);
        }
        public async Task<IResult> CreateSubcategory(Subcategory subcategory)
        {
            var newSubcategory = new Subcategory
            {
                SubcategoryName = subcategory.SubcategoryName,
                CategoryId = subcategory.CategoryId
            };
            try
            {
                _context.Subcategories.Add(newSubcategory);
                await _context.SaveChangesAsync();
            } catch (Exception ex) { return Results.BadRequest(ex.InnerException?.Message); }

            return Results.Ok(newSubcategory);
        }
        public async Task<IResult> UpdateSubcategoryDetails(Subcategory update, int id)
        {
            var retrievedCategory = _context.Subcategories.FirstOrDefault(c => c.Id == id);
            if (retrievedCategory != null)
            {
                retrievedCategory.SubcategoryName = update.SubcategoryName;
                retrievedCategory.CategoryId = update.CategoryId;
                try
                {
                    _context.Subcategories.Update(retrievedCategory);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.InnerException?.Message);
                }
                return Results.Ok(retrievedCategory);
            }
            else
            {
                return Results.NotFound($"Subcategory with ID = {id} was not found");
            }

        }
        public async Task<IResult> RemoveSubcategory(int id)
        {
            var subcategory = _context.Subcategories.FirstOrDefault(c => c.Id == id);
            if (subcategory != null)
            {
                _context.Subcategories.Remove(subcategory);
                await _context.SaveChangesAsync();
                return Results.Ok(subcategory);
            }
            else
            {
                return Results.NotFound($"Category with CategoryID = {id} was not found");
            }
        }
    }
}
