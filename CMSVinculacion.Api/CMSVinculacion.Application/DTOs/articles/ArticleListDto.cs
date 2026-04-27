namespace CMSVinculacion.Application.DTOs.articles
{
    public class ArticleListDto
    {
        public string Id { get; set; } = string.Empty;  // string para compatibilidad con front
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }           // mapeado desde FeaturedImage
        public string? Emoji { get; set; }
        public string? Excerpt { get; set; }
        public int ReadingTime { get; set; }
        public bool Featured { get; set; }
        public string? Category { get; set; }           // primera categoria
        public List<string> Categories { get; set; } = new();
        public string? StatusName { get; set; }
        public string Date { get; set; } = string.Empty;
        public DateTime? PublishedAt { get; set; }
    }
}