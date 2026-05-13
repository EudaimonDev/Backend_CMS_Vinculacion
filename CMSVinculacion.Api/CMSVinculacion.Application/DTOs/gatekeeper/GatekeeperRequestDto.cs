using System.ComponentModel.DataAnnotations;

namespace CMSVinculacion.Application.DTOs.gatekeeper
{
    public class GatekeeperRequestDto
    {
        [MaxLength(200)]
        public string? Nombres { get; set; }

        [Required]
        [Range(1, 120)]
        public int Edad { get; set; }

        [Required, MaxLength(20)]
        public string Sexo { get; set; } = string.Empty;
    }
}