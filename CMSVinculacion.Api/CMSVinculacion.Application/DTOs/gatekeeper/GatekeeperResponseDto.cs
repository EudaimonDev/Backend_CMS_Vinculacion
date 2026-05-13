namespace CMSVinculacion.Application.DTOs.gatekeeper
{
    public class GatekeeperResponseDto
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string? Token { get; set; }
        public int ExpiresIn { get; set; } = 2592000;
        public GatekeeperVisitorDto? Visitor { get; set; }
    }

    public class GatekeeperVisitorDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
    }
}