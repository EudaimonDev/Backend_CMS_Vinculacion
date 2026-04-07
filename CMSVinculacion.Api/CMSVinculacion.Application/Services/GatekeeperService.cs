using CMSVinculacion.Application.DTOs.gatekeeper;
using CMSVinculacion.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CMSVinculacion.Domain.Entities.Catalogos;

namespace CMSVinculacion.Application.Services
{
    public class GatekeeperService : IGatekeeperService
    {
        private readonly IGatekeeperRepository _repo;
        private const int HorasExpiracion = 24;

        public GatekeeperService(IGatekeeperRepository repo)
        {
            _repo = repo;
        }

        public async Task<GatekeeperResponseDto> RegistrarVisitanteAsync(
            GatekeeperRequestDto request, string ipAddress)
        {
            // Sanitizar entradas
            var nombre = SanitizarTexto(request.Nombre);
            var email = SanitizarTexto(request.Email).ToLower();
            var institucion = SanitizarTexto(request.Institucion);

            // Validaciones básicas
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
            var expiracion = DateTime.UtcNow.AddHours(HorasExpiracion);

            var visitante = new VisitanteAcceso
            {
                Nombre = nombre,
                Email = email,
                Institucion = institucion,
                IpAddress = ipAddress,
                Token = token,
                FechaRegistro = DateTime.UtcNow,
                FechaExpiracion = expiracion,
                Active = true
            };

            await _repo.GuardarVisitanteAsync(visitante);

            return new GatekeeperResponseDto
            {
                Exito = true,
                Mensaje = "Registro exitoso. Bienvenido.",
                Token = token,
                Expiracion = expiracion
            };
        }

        public async Task<GatekeeperResponseDto> ValidarTokenAsync(string token)
        {
            var visitante = await _repo.ObtenerPorTokenAsync(token);

            if (visitante is null || !visitante.Active)
                return new GatekeeperResponseDto
                {
                    Exito = false,
                    Mensaje = "Token inválido o expirado."
                };

            if (visitante.FechaExpiracion < DateTime.UtcNow)
                return new GatekeeperResponseDto
                {
                    Exito = false,
                    Mensaje = "El token ha expirado. Por favor regístrese nuevamente."
                };

            return new GatekeeperResponseDto
            {
                Exito = true,
                Mensaje = "Token válido.",
                Token = token,
                Expiracion = visitante.FechaExpiracion
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
