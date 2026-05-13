using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSVinculacion.Domain.Entities.Gatekeeper
{
    [Table(nameof(Visitors), Schema = "GAT")]
    public class Visitors
    {
        [Key]
        public int VisitorId { get; set; }

        [MaxLength(200)]
        public string? Nombres { get; set; }

        public int Edad { get; set; }

        [MaxLength(20)]
        public string Sexo { get; set; } = string.Empty;

        [MaxLength(512)]
        public string? CookieToken { get; set; }

        [MaxLength(50)]
        public string? IPAddress { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}