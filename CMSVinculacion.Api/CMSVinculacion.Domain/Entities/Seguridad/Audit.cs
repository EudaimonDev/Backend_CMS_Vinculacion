using System.ComponentModel.DataAnnotations;

namespace CMSVinculacion.Domain.Entities.Seguridad
{
    public class Audit
    {
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [MaxLength(307)]
        public string? CreatedBy { get; set; }
        [MaxLength(100)]
        public string? CreatedIp { get; set; }


        public DateTime? UpdatedAt { get; set; }
        [MaxLength(307)]
        public string? UpdatedBy { get; set; }
        [MaxLength(100)]
        public string? UpdatedIp { get; set; }


        public DateTime? DeletedAt { get; set; }
        [MaxLength(307)]
        public string? DeletedBy { get; set; }
        [MaxLength(100)]
        public string? DeletedIp { get; set; }
    }
}
