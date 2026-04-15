using CMSVinculacion.Application.Interfaces;
using CMSVinculacion.Domain.Entities.Contenido;
using Microsoft.EntityFrameworkCore;

namespace CMSVinculacion.Infrastructure.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly SqlDbContext _context;

        public ArticleRepository(SqlDbContext context) => _context = context;

        public async Task<Articles?> GetByIdAsync(int id, bool includeDeleted = false)
        {
            var query = _context.Articles
                .Include(a => a.Status)
                .Include(a => a.Author)
                .Include(a => a.ArticleCategories)!.ThenInclude(ac => ac.Category)
                .Include(a => a.MediaFiles)
                .Where(a => a.ArticleId == id);

            if (!includeDeleted)
                query = query.Where(a => a.DeletedAt == null);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<Articles> Items, int Total)> GetPublishedPagedAsync(
            int page, int pageSize, int? categoryId = null)
        {
            var query = _context.Articles
                .Include(a => a.Status)
                .Include(a => a.ArticleCategories)!.ThenInclude(ac => ac.Category)
                .Where(a => a.DeletedAt == null && a.Status!.StatusName == "Published");

            if (categoryId.HasValue)
                query = query.Where(a => a.ArticleCategories!.Any(ac => ac.CategoryId == categoryId));

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(a => a.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<IEnumerable<Articles>> GetRecentPublishedAsync(int count = 5) =>
            await _context.Articles
                .Include(a => a.ArticleCategories)!.ThenInclude(ac => ac.Category)
                .Where(a => a.DeletedAt == null && a.Status!.StatusName == "Published")
                .OrderByDescending(a => a.PublishedAt)
                .Take(count)
                .ToListAsync();

        public async Task<IEnumerable<Articles>> GetGalleryAsync() =>
            await _context.Articles
                .Include(a => a.ArticleCategories)!.ThenInclude(ac => ac.Category)
                .Where(a => a.DeletedAt == null && a.Status!.StatusName == "Published"
                         && a.FeaturedImage != null)
                .OrderByDescending(a => a.PublishedAt)
                .ToListAsync();

        public async Task<IEnumerable<Articles>> GetAllAdminAsync(
            int? statusId, int? categoryId, int page, int pageSize) =>
            await _context.Articles
                .Include(a => a.Status)
                .Include(a => a.Author)
                .Include(a => a.ArticleCategories)!.ThenInclude(ac => ac.Category)
                .Where(a => a.DeletedAt == null
                    && (!statusId.HasValue || a.StatusId == statusId)
                    && (!categoryId.HasValue || a.ArticleCategories!.Any(ac => ac.CategoryId == categoryId)))
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<Articles> CreateAsync(Articles article)
        {
            await _context.Articles.AddAsync(article);
            await _context.SaveChangesAsync();
            return article;
        }

        public async Task UpdateAsync(Articles article)
        {
            _context.Articles.Update(article);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id, string deletedBy)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article is null) return;
            article.DeletedAt = DateTime.UtcNow;
            article.DeletedBy = deletedBy;
            article.IsActive = false;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, int statusId, string updatedBy)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article is null) return;
            article.StatusId = statusId;
            article.UpdatedAt = DateTime.UtcNow;
            article.UpdatedBy = updatedBy;
            if (statusId == 2) // Published
                article.PublishedAt ??= DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}