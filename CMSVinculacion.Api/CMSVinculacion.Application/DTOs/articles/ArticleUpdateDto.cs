using System.ComponentModel.DataAnnotations;

namespace CMSVinculacion.Application.DTOs.articles
{
    public class ArticleUpdateDto
    {
        [Required, MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(350)]
        public string? Slug { get; set; }

        [Required]
        public string ContentHtml { get; set; } = string.Empty;

        public string? FeaturedImage { get; set; }

        [MaxLength(10)]
        public string? Emoji { get; set; }

        [MaxLength(500)]
        public string? Excerpt { get; set; }

        public int ReadingTime { get; set; } = 1;

        public bool Featured { get; set; } = false;

        public List<int> CategoryIds { get; set; } = new();
    }
}