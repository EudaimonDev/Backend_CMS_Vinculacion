using CMSVinculacion.Application.Interfaces;
using CMSVinculacion.Domain.Entities.Catalogos;
using CMSVinculacion.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CMSVinculacion.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly SqlDbContext _context;

        public CategoryRepository(SqlDbContext context) => _context = context;

        public async Task<IEnumerable<Categories>> GetAllAsync(bool onlyPublic = false)
        {
            var query = _context.Categories
                .Include(c => c.ArticleCategories)
                .Include(c => c.SubCategories!.Where(s => s.DeletedAt == null))
                .Where(c => c.DeletedAt == null);

            if (onlyPublic)
            {
                query = query.Where(c =>
                    c.IsActive &&
                    c.IsPublicVisible &&
                    c.Estado == EnumEstado.Publicado);
            }

            return await query
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Categories?> GetByIdAsync(int id, bool includeInactive = false)
        {
            var query = _context.Categories
                .Include(c => c.ArticleCategories)
                .Include(c => c.SubCategories!.Where(s => s.DeletedAt == null))
                .Where(c => c.CategoryId == id && c.DeletedAt == null);

            if (!includeInactive)
                query = query.Where(c => c.IsActive);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<Categories?> GetBySlugAsync(string slug) =>
            await _context.Categories
                .Include(c => c.SubCategories!.Where(s => s.DeletedAt == null && s.IsActive))
                .FirstOrDefaultAsync(c =>
                    c.Slug == slug &&
                    c.DeletedAt == null &&
                    c.IsActive &&
                    c.IsPublicVisible &&
                    c.Estado == EnumEstado.Publicado);

        public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null) =>
            await _context.Categories
                .AnyAsync(c =>
                    c.Slug == slug &&
                    c.DeletedAt == null &&
                    (!excludeId.HasValue || c.CategoryId != excludeId));

        public async Task<bool> SubCategorySlugExistsAsync(int categoryId, string slug, int? excludeId = null) =>
            await _context.SubCategories
                .AnyAsync(s =>
                    s.CategoryId == categoryId &&
                    s.Slug == slug &&
                    s.DeletedAt == null &&
                    (!excludeId.HasValue || s.SubCategoryId != excludeId));

        public async Task<bool> HasArticlesAsync(int id) =>
            await _context.ArticleCategories.AnyAsync(ac => ac.CategoryId == id);

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
            var cat = await _context.Categories
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (cat is null) return;

            cat.IsActive = false;
            cat.Estado = EnumEstado.Archivado;
            cat.DeletedAt = DateTime.UtcNow;

            if (cat.SubCategories != null)
            {
                foreach (var sub in cat.SubCategories.Where(s => s.DeletedAt == null))
                {
                    sub.IsActive = false;
                    sub.Estado = EnumEstado.Archivado;
                    sub.DeletedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
