using CMSVinculacion.Domain.Entities.Contenido;
using CMSVinculacion.Domain.Entities.Seguridad;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSVinculacion.Domain.Entities.Catalogos
{
    [Table(nameof(Categories), Schema = "CAT")]
    public class Categories : Audit
    {
        [Key]
        public int CategoryId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(120)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Description { get; set; }

        public bool IsPublicVisible { get; set; } = true;

        // Relación N:M con Articles via tabla intermedia
        public ICollection<ArticleCategory>? ArticleCategories { get; set; }
    }
}