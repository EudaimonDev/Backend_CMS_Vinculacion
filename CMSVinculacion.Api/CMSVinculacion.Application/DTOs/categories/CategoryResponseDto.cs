namespace CMSVinculacion.Application.DTOs.categories
{
    public class CategoryResponseDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsPublicVisible { get; set; }
        public int ArticleCount { get; set; }
    }
}