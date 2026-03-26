using CMSVinculacion.Domain.Entities.Seguridad;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSVinculacion.Domain.Entities.Catalogos
{
    [Table(nameof(Persona), Schema ="CAT")]
    public class Persona : Audit
    {
        [Key]
        public long IdPersona { get; set; }
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
    }
}
