using CMSVinculacion.Domain.Entities.Seguridad;
using CMSVinculacion.Domain.Enums;
using CMSVinculacion.Domain.Entities.Contenido;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSVinculacion.Domain.Entities.Catalogos
{
    [Table(nameof(SubCategories), Schema = "CAT")]
    public class SubCategories : Audit
    {
        [Key]
        public int SubCategoryId { get; set; }

        public int CategoryId { get; set; }
        public Categories? Category { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(120)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Description { get; set; }

        public EnumEstado Estado { get; set; } = EnumEstado.Borrador;

        public ICollection<Articles>? Articles { get; set; }
    }
}
