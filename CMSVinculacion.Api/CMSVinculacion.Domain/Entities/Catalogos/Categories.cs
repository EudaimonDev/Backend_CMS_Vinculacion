using CMSVinculacion.Domain.Entities.Contenido;
using CMSVinculacion.Domain.Entities.Seguridad;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string? Description { get; set; } = string.Empty;

        public ICollection<Articles>? Articles { get; set; }

    }
}
