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

            // Tags permitidos
            _sanitizer.AllowedTags.Clear();
            _sanitizer.AllowedTags.UnionWith(new[]
            {
                "p", "h1", "h2", "h3", "h4", "strong", "em", "u",
                "ul", "ol", "li", "img", "a", "div", "figure",
                "figcaption", "iframe", "blockquote", "br", "span"
            });

            // Atributos permitidos
            _sanitizer.AllowedAttributes.Clear();
            _sanitizer.AllowedAttributes.UnionWith(new[]
            {
                "src", "alt", "href", "style", "class", "width", "height",
                "frameborder", "allowfullscreen", "allow", "title",
                "target", "rel"
            });

            // Esquemas permitidos
            _sanitizer.AllowedSchemes.Clear();
            _sanitizer.AllowedSchemes.Add("https");
            _sanitizer.AllowedSchemes.Add("http");
            _sanitizer.AllowedSchemes.Add("data");

            // No filtrar URLs externas en src de imágenes
            _sanitizer.FilterUrl += (sender, args) =>
            {
                args.SanitizedUrl = args.OriginalUrl;
            };

            _sanitizer.AllowedAtRules.Clear();

            // CSS permitido
            _sanitizer.AllowedCssProperties.Add("background-image");
            _sanitizer.AllowedCssProperties.Add("background-size");
            _sanitizer.AllowedCssProperties.Add("background-position");
            _sanitizer.AllowedCssProperties.Add("background-color");
            _sanitizer.AllowedCssProperties.Add("background");
            _sanitizer.AllowedCssProperties.Add("padding");
            _sanitizer.AllowedCssProperties.Add("padding-bottom");
            _sanitizer.AllowedCssProperties.Add("border-radius");
            _sanitizer.AllowedCssProperties.Add("margin");
            _sanitizer.AllowedCssProperties.Add("margin-bottom");
            _sanitizer.AllowedCssProperties.Add("margin-top");
            _sanitizer.AllowedCssProperties.Add("position");
            _sanitizer.AllowedCssProperties.Add("top");
            _sanitizer.AllowedCssProperties.Add("left");
            _sanitizer.AllowedCssProperties.Add("width");
            _sanitizer.AllowedCssProperties.Add("height");
            _sanitizer.AllowedCssProperties.Add("min-height");
            _sanitizer.AllowedCssProperties.Add("overflow");
            _sanitizer.AllowedCssProperties.Add("color");
            _sanitizer.AllowedCssProperties.Add("text-shadow");
            _sanitizer.AllowedCssProperties.Add("text-align");
            _sanitizer.AllowedCssProperties.Add("text-decoration");
            _sanitizer.AllowedCssProperties.Add("display");
            _sanitizer.AllowedCssProperties.Add("grid-template-columns");
            _sanitizer.AllowedCssProperties.Add("gap");
            _sanitizer.AllowedCssProperties.Add("max-width");
            _sanitizer.AllowedCssProperties.Add("font-size");
            _sanitizer.AllowedCssProperties.Add("font-weight");
            _sanitizer.AllowedCssProperties.Add("object-fit");
            _sanitizer.AllowedCssProperties.Add("z-index");
            _sanitizer.AllowedCssProperties.Add("border");
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
            int? statusId, int? categoryId, DateTime? startDate, int page, int pageSize) =>
            (await _repo.GetAllAdminAsync(statusId, categoryId, startDate, page, pageSize)).Select(ToListDto);

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
                Emoji = dto.Emoji ?? string.Empty,
                Excerpt = dto.Excerpt,
                ReadingTime = dto.ReadingTime,
                Featured = dto.Featured,
                ContentHtml = _sanitizer.Sanitize(dto.ContentHtml),
                BlocksJson = dto.BlocksJson,
                FeaturedImage = dto.FeaturedImage,
                AuthorId = authorId,
                StatusId = 1,
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

            article.Title = dto.Title;
            article.Slug = dto.Slug ?? GenerateSlug(dto.Title);
            article.Emoji = dto.Emoji ?? string.Empty;
            article.Excerpt = dto.Excerpt;
            article.ReadingTime = dto.ReadingTime;
            article.Featured = dto.Featured;
            article.ContentHtml = _sanitizer.Sanitize(dto.ContentHtml);
            article.BlocksJson = dto.BlocksJson;
            article.FeaturedImage = dto.FeaturedImage;
            article.UpdatedAt = DateTime.UtcNow;
            article.UpdatedBy = updatedBy;

            await _repo.UpdateAsync(article);
            await _repo.UpdateCategoriesAsync(id, dto.CategoryIds);

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
            Id = a.ArticleId.ToString(),
            Title = a.Title,
            Slug = a.Slug,
            ImageUrl = a.FeaturedImage,
            Emoji = a.Emoji,
            Excerpt = a.Excerpt,
            ReadingTime = a.ReadingTime,
            Featured = a.Featured,
            Category = a.ArticleCategories?.FirstOrDefault()?.Category?.Slug?.ToLower(),
            Categories = a.ArticleCategories?.Select(ac => ac.Category?.Name ?? "").ToList() ?? new(),
            StatusName = a.Status?.StatusName,
            Date = a.PublishedAt.HasValue
                ? a.PublishedAt.Value.ToString("d MMM yyyy", new System.Globalization.CultureInfo("es-ES"))
                : a.CreatedAt.ToString("d MMM yyyy", new System.Globalization.CultureInfo("es-ES")),
            PublishedAt = a.PublishedAt
        };

        private static ArticleResponseDto ToResponseDto(Articles a) => new()
        {
            Id = a.ArticleId.ToString(),
            Title = a.Title,
            Slug = a.Slug,
            ContentHtml = a.ContentHtml,
            BlocksJson = a.BlocksJson,
            ImageUrl = a.FeaturedImage,
            Emoji = a.Emoji,
            Excerpt = a.Excerpt,
            ReadingTime = a.ReadingTime,
            Featured = a.Featured,
            Category = a.ArticleCategories?.FirstOrDefault()?.Category?.Slug?.ToLower(),
            Categories = a.ArticleCategories?.Select(ac => ac.Category?.Name ?? "").ToList() ?? new(),
            StatusName = a.Status?.StatusName,
            AuthorUsername = a.Author?.Username,
            Date = a.PublishedAt.HasValue
                ? a.PublishedAt.Value.ToString("d MMM yyyy", new System.Globalization.CultureInfo("es-ES"))
                : a.CreatedAt.ToString("d MMM yyyy", new System.Globalization.CultureInfo("es-ES")),
            PublishedAt = a.PublishedAt,
            ViewCount = a.ViewCount,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        };
    }
}