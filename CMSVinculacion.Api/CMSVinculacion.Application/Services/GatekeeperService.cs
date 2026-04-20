using CMSVinculacion.Application.DTOs.gatekeeper;
using CMSVinculacion.Application.Interfaces;
using CMSVinculacion.Domain.Entities.Gatekeeper;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

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
        if (request.Edad <= 0 || string.IsNullOrEmpty(request.Sexo))
        {
            return new GatekeeperResponseDto
            {
                Exito = false,
                Mensaje = "edad y sexo son obligatorios"
            };
        }

        var dataJson = request.Extras != null && request.Extras.Count > 0
            ? JsonSerializer.Serialize(request.Extras)
            : null;

        var token = GenerarToken(ipAddress);

        var visitante = new Visitors
        {
            Edad = request.Edad,
            Sexo = request.Sexo,
            DataJson = dataJson,
            IPAddress = ipAddress,
            CookieToken = token,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _repo.GuardarVisitanteAsync(visitante);

        return new GatekeeperResponseDto
        {
            Exito = true,
            Mensaje = "Registro exitoso",
            Token = token
        };
    }

    public async Task<GatekeeperResponseDto> ValidarTokenAsync(string token)
    {
        var visitante = await _repo.ObtenerPorTokenAsync(token);

        if (visitante is null || !visitante.IsActive)
        {
            return new GatekeeperResponseDto
            {
                Exito = false,
                Mensaje = "Token inválido"
            };
        }

        return new GatekeeperResponseDto
        {
            Exito = true,
            Mensaje = "Token válido",
            Token = token
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