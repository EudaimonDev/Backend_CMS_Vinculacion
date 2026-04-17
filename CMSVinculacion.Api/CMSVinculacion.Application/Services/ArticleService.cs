using CMSVinculacion.Application.DTOs.articles;
using CMSVinculacion.Application.Interfaces;
using CMSVinculacion.Domain.Entities.Contenido;
using Ganss.Xss;

namespace CMSVinculacion.Application.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _repo;
        private readonly HtmlSanitizer _sanitizer;

        public ArticleService(IArticleRepository repo)
        {
            _repo = repo;
            _sanitizer = new HtmlSanitizer();
            _sanitizer.AllowedTags.UnionWith(new[] { "p", "h1", "h2", "h3", "strong", "em", "ul", "ol", "li", "img", "a" });
        }

        public async Task<(IEnumerable<ArticleListDto> Items, int Total)> GetPublishedAsync(
            int page, int pageSize, int? categoryId = null)
        {
            var (items, total) = await _repo.GetPublishedPagedAsync(page, pageSize, categoryId);
            return (items.Select(ToListDto), total);
        }

        public async Task<ArticleResponseDto?> GetPublishedByIdAsync(int id)
        {
            var a = await _repo.GetByIdAsync(id);
            return a?.Status?.StatusName == "Published" ? ToResponseDto(a) : null;
        }

        public async Task<IEnumerable<ArticleListDto>> GetRecentAsync(int count = 5) =>
            (await _repo.GetRecentPublishedAsync(count)).Select(ToListDto);

        public async Task<IEnumerable<ArticleListDto>> GetGalleryAsync() =>
            (await _repo.GetGalleryAsync()).Select(ToListDto);

        public async Task<IEnumerable<ArticleListDto>> GetAllAdminAsync(
            int? statusId, int? categoryId, int page, int pageSize) =>
            (await _repo.GetAllAdminAsync(statusId, categoryId, page, pageSize)).Select(ToListDto);

        public async Task<ArticleResponseDto?> GetAdminByIdAsync(int id)
        {
            var a = await _repo.GetByIdAsync(id, includeDeleted: true);
            return a is null ? null : ToResponseDto(a);
        }

        public async Task<ArticleResponseDto> CreateAsync(ArticleCreateDto dto, int authorId)
        {
            var article = new Articles
            {
                Title = dto.Title,
                Slug = dto.Slug ?? GenerateSlug(dto.Title),
                ContentHtml = _sanitizer.Sanitize(dto.ContentHtml),
                FeaturedImage = dto.FeaturedImage,
                AuthorId = authorId,
                StatusId = 1, // Draft
                CreatedAt = DateTime.UtcNow,
                ArticleCategories = dto.CategoryIds
                    .Select(cid => new ArticleCategory { CategoryId = cid })
                    .ToList()
            };
            var created = await _repo.CreateAsync(article);
            return ToResponseDto(created);
        }

        public async Task<ArticleResponseDto?> UpdateAsync(int id, ArticleUpdateDto dto, string updatedBy)
        {
            var article = await _repo.GetByIdAsync(id);
            if (article is null) return null;
            //actualizamos solo los datos del articulo
            article.Title = dto.Title;
            article.Slug = dto.Slug ?? GenerateSlug(dto.Title);
            article.ContentHtml = _sanitizer.Sanitize(dto.ContentHtml);
            article.FeaturedImage = dto.FeaturedImage;
            article.UpdatedAt = DateTime.UtcNow;
            article.UpdatedBy = updatedBy;
            

            await _repo.UpdateAsync(article);
            //actualizamos las categorias
            await _repo.UpdateCategoriesAsync(id,dto.CategoryIds);
            //llamamos a los articulos con sus nuevas categorias
            var updatedArticle = await _repo.GetByIdAsync(id);
            return ToResponseDto(updatedArticle!);
        }

        public async Task<bool> DeleteAsync(int id, string deletedBy)
        {
            var article = await _repo.GetByIdAsync(id);
            if (article is null) return false;
            await _repo.SoftDeleteAsync(id, deletedBy);
            return true;
        }

        public async Task<bool> UpdateStatusAsync(int id, ArticleStatusUpdateDto dto, string updatedBy)
        {
            var article = await _repo.GetByIdAsync(id);
            if (article is null) return false;
            await _repo.UpdateStatusAsync(id, dto.StatusId, updatedBy);
            return true;
        }

        private static string GenerateSlug(string title) =>
            title.ToLower().Replace(" ", "-")
                .Replace("á", "a").Replace("é", "e").Replace("í", "i")
                .Replace("ó", "o").Replace("ú", "u").Replace("ñ", "n");

        private static ArticleListDto ToListDto(Articles a) => new()
        {
            ArticleId = a.ArticleId,
            Title = a.Title,
            Slug = a.Slug,
            FeaturedImage = a.FeaturedImage,
            StatusName = a.Status?.StatusName,
            Categories = a.ArticleCategories?.Select(ac => ac.Category?.Name ?? "").ToList() ?? new(),
            PublishedAt = a.PublishedAt
        };

        private static ArticleResponseDto ToResponseDto(Articles a) => new()
        {
            ArticleId = a.ArticleId,
            Title = a.Title,
            Slug = a.Slug,
            ContentHtml = a.ContentHtml,
            FeaturedImage = a.FeaturedImage,
            StatusName = a.Status?.StatusName,
            AuthorUsername = a.Author?.Username,
            Categories = a.ArticleCategories?.Select(ac => ac.Category?.Name ?? "").ToList() ?? new(),
            PublishedAt = a.PublishedAt,
            ViewCount = a.ViewCount,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        };
    }
}