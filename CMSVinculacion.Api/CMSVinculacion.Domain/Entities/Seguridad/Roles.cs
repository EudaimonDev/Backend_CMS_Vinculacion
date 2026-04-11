using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSVinculacion.Domain.Entities.Seguridad
{
    [Table(nameof(Roles), Schema = "SEG")]
    public class Roles
    {
        [Key]
        public int RoleId { get; set; }
        [MaxLength(50)]
        public string RoleName { get; set; } = string.Empty;
        [MaxLength (200)]
        public string? Description { get; set; } = string.Empty;

        public ICollection<Users>? Users { get; set; }
    }
}
