namespace CMSVinculacion.Application.DTOs.articles
{
    public class ArticleResponseDto
    {
        public int ArticleId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string ContentHtml { get; set; } = string.Empty;
        public string? FeaturedImage { get; set; }
        public string? StatusName { get; set; }
        public string? AuthorUsername { get; set; }
        public List<string> Categories { get; set; } = new();
        public DateTime? PublishedAt { get; set; }
        public int ViewCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}