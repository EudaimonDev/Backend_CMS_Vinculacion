using System.ComponentModel.DataAnnotations;

namespace CMSVinculacion.Application.DTOs.articles
{
    public class ArticleCreateDto
    {
        [Required, MaxLength(300)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(350)]
        public string? Slug { get; set; }
        [Required]
        public string ContentHtml { get; set; } = string.Empty;
        public string? FeaturedImage { get; set; }
        public List<int> CategoryIds { get; set; } = new();
    }
}