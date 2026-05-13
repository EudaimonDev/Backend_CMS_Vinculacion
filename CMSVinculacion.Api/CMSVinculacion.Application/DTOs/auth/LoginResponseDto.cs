namespace CMSVinculacion.Application.DTOs.auth
{
    public class LoginResponseDto
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string? Token { get; set; }          // "token" que espera el front
        public AuthUserDto? User { get; set; }      // "user" que espera el front
        public string? RefreshToken { get; set; }
        public DateTime? Expiration { get; set; }
    }

    public class AuthUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}