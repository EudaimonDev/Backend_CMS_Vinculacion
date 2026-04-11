using CMSVinculacion.Domain.Entities.Contenido;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSVinculacion.Domain.Entities.Seguridad
{

        [Table(nameof(Users), Schema = "SEG")]
        public class Users : Audit
        {
            [Key]
            public int UserId { get; set; }
            [MaxLength(100)]
            public string Username { get; set; } = string.Empty;
            [MaxLength(200)]
            public string Email { get; set; } = string.Empty;
            [MaxLength(512)]
            public string PasswordHash { get; set; } = string.Empty;

            public int? RoleId { get; set; }
            public Roles? Role { get; set; }

            public DateTime? LastLogin { get; set; }

            public ICollection<Articles>? Articles { get; set; }
            public ICollection<AuditLog>? Logs { get; set; }

    }
}
