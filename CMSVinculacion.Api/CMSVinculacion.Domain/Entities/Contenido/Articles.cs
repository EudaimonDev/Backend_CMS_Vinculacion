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

        public int? StatusId { get; set; }
        public ArticleStatus? Status { get; set; }

        public int? AuthorId { get; set; }
        public Users? Author { get; set; }

        public DateTime? PublishedAt { get; set; }

        public int ViewCount { get; set; }

        // Relación N:M con Categories via tabla intermedia
        public ICollection<ArticleCategory>? ArticleCategories { get; set; }

        public ICollection<MediaFiles>? MediaFiles { get; set; }
    }
}