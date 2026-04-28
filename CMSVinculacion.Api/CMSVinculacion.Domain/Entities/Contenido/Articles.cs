using CMSVinculacion.Domain.Entities.Seguridad;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSVinculacion.Domain.Entities.Contenido
{
    [Table(nameof(Articles), Schema = "CON")]
    public class Articles : Audit
    {
        [Key]
        public int ArticleId { get; set; }

        [MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(350)]
        public string Slug { get; set; } = string.Empty;

        public string ContentHtml { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? FeaturedImage { get; set; }

        // Campo emoji para el front
        [MaxLength(10)]
        public string? Emoji { get; set; }

        // Extracto/resumen del artículo
        [MaxLength(500)]
        public string? Excerpt { get; set; }

        // Tiempo de lectura estimado en minutos
        public int ReadingTime { get; set; } = 1;

        // Artículo destacado (hero)
        public bool Featured { get; set; } = false;

        public int? StatusId { get; set; }
        public ArticleStatus? Status { get; set; }

        public int? AuthorId { get; set; }
        public Users? Author { get; set; }

        public DateTime? PublishedAt { get; set; }

        public int ViewCount { get; set; }

        public ICollection<ArticleCategory>? ArticleCategories { get; set; }
        public ICollection<MediaFiles>? MediaFiles { get; set; }
    }
}