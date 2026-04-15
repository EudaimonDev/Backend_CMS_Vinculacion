using CMSVinculacion.Application.Interfaces;
using CMSVinculacion.Domain.Entities.Catalogos;
using Microsoft.EntityFrameworkCore;

namespace CMSVinculacion.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly SqlDbContext _context;

        public CategoryRepository(SqlDbContext context) => _context = context;

        public async Task<IEnumerable<Categories>> GetAllAsync(bool onlyPublic = false) =>
            await _context.Categories
                .Include(c => c.ArticleCategories)
                .Where(c => c.IsActive && (!onlyPublic || c.IsPublicVisible))
                .ToListAsync();

        public async Task<Categories?> GetByIdAsync(int id) =>
            await _context.Categories
                .Include(c => c.ArticleCategories)
                .FirstOrDefaultAsync(c => c.CategoryId == id && c.IsActive);

        public async Task<Categories?> GetBySlugAsync(string slug) =>
            await _context.Categories
                .FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive);

        public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null) =>
            await _context.Categories
                .AnyAsync(c => c.Slug == slug && c.IsActive
                    && (!excludeId.HasValue || c.CategoryId != excludeId));

        public async Task<bool> HasArticlesAsync(int id) =>
            await _context.ArticleCategories
                .AnyAsync(ac => ac.CategoryId == id);

        public async Task<Categories> CreateAsync(Categories category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task UpdateAsync(Categories category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var cat = await _context.Categories.FindAsync(id);
            if (cat is null) return;
            cat.IsActive = false;
            cat.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}