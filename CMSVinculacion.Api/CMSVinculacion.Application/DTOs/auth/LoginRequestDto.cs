using System.ComponentModel.DataAnnotations;

namespace CMSVinculacion.Application.DTOs.auth
{
    public class LoginRequestDto
    {
        [Required, MaxLength(200)]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}