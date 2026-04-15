namespace CMSVinculacion.Application.DTOs.articles
{
    public class ArticleListDto
    {
        public int ArticleId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? FeaturedImage { get; set; }
        public string? StatusName { get; set; }
        public List<string> Categories { get; set; } = new();
        public DateTime? PublishedAt { get; set; }
    }
}