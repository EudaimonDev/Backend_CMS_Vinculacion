using CMSVinculacion.Domain.Entities.Seguridad;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSVinculacion.Domain.Entities.Gatekeeper
{
    [Table(nameof(Visitors), Schema = "GAT")]
    public class Visitors : Audit
    {
        [Key]
        public int VisitorId { get; set; }
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;
        [MaxLength(200)]
        public string? Email { get; set; } = string.Empty;
        [MaxLength (300)]
        public string? Institution { get; set; } = string.Empty;

        [MaxLength(512)]
        public string? CookieToken { get; set; } = string.Empty;
        [MaxLength(50)]
        public string? IPAddress { get; set; }

        
    }
}
