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

        public GatekeeperService(IGatekeeperRepository repo)
        {
            _repo = repo;
        }

        public async Task<GatekeeperResponseDto> RegistrarVisitanteAsync(
            GatekeeperRequestDto request, string ipAddress)
        {
            var nombre = SanitizarTexto(request.Nombre);
            var email = SanitizarTexto(request.Email).ToLower();
            var institucion = SanitizarTexto(request.Institucion);

            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(institucion))
            {
                return new GatekeeperResponseDto
                {
                    Exito = false,
                    Mensaje = "Todos los campos son obligatorios."
                };
            }

            if (!email.Contains('@'))
            {
                return new GatekeeperResponseDto
                {
                    Exito = false,
                    Mensaje = "El email ingresado no es válido."
                };
            }

            var token = GenerarToken(email, ipAddress);

            var visitante = new Visitors
            {
                FullName = nombre,
                Email = email,
                Institution = institucion,
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
                Token = token
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
                Token = token
            };
        }

        private static string GenerarToken(string email, string ip)
        {
            var raw = $"{email}:{ip}:{Guid.NewGuid()}:{DateTime.UtcNow.Ticks}";
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
            return Convert.ToBase64String(bytes)
                .Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        private static string SanitizarTexto(string input) =>
            System.Net.WebUtility.HtmlEncode(input.Trim());
    }
}