using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSVinculacion.Domain.Entities.Seguridad
{
    [Table(nameof(AuditLog), Schema = "SEG")]
    public class AuditLog : Audit
    {
        [Key]
        public int LogId { get; set; }
        public int? UserId { get; set; }
        [MaxLength(200)]
        public string Action { get; set; } = string.Empty;
        [MaxLength(100)]
        public string? Entity { get; set; } = string.Empty;

        public int? EntityId { get; set; }

        public string? Detail { get; set; } = string.Empty;
        [MaxLength(50)]
        public string? IPAddress { get; set; } = string.Empty;


        public Users? User { get; set; }

    }
}
