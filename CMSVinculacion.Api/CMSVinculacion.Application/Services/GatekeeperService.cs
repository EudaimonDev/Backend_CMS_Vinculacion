using CMSVinculacion.Application.DTOs.gatekeeper;
using CMSVinculacion.Application.Interfaces;
using CMSVinculacion.Domain.Entities.Gatekeeper;
using System.Security.Cryptography;
using System.Text;

namespace CMSVinculacion.Application.Services
{
    public class GatekeeperService : IGatekeeperService
    {
        private readonly IGatekeeperRepository _repo;

        public GatekeeperService(IGatekeeperRepository repo) => _repo = repo;

        public async Task<GatekeeperResponseDto> RegistrarVisitanteAsync(
            GatekeeperRequestDto request, string ipAddress)
        {
            if (request.Edad <= 0 || string.IsNullOrWhiteSpace(request.Sexo))
            {
                return new GatekeeperResponseDto
                {
                    Exito = false,
                    Mensaje = "Edad y sexo son obligatorios."
                };
            }

            var token = GenerarToken(ipAddress);

            var visitante = new Visitors
            {
                Nombres = request.Nombres?.Trim(),
                Edad = request.Edad,
                Sexo = request.Sexo.Trim(),
                IPAddress = ipAddress,
                CookieToken = token,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _repo.GuardarVisitanteAsync(visitante);

            return new GatekeeperResponseDto
            {
                Exito = true,
                Mensaje = "Registro exitoso. Bienvenido.",
                Token = token,
                ExpiresIn = 2592000,
                Visitor = new GatekeeperVisitorDto
                {
                    Id = visitante.VisitorId.ToString(),
                    Nombres = visitante.Nombres ?? "Visitante"
                }
            };
        }

        public async Task<GatekeeperResponseDto> ValidarTokenAsync(string token)
        {
            var visitante = await _repo.ObtenerPorTokenAsync(token);

            if (visitante is null || !visitante.IsActive)
                return new GatekeeperResponseDto
                {
                    Exito = false,
                    Mensaje = "Token inválido o visitante inactivo."
                };

            return new GatekeeperResponseDto
            {
                Exito = true,
                Mensaje = "Token válido.",
                Token = token,
                ExpiresIn = 2592000,
                Visitor = new GatekeeperVisitorDto
                {
                    Id = visitante.VisitorId.ToString(),
                    Nombres = visitante.Nombres ?? "Visitante"
                }
            };
        }

        private static string GenerarToken(string ip)
        {
            var raw = $"{ip}:{Guid.NewGuid()}:{DateTime.UtcNow.Ticks}";
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
            return Convert.ToBase64String(bytes)
                .Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
    }
}