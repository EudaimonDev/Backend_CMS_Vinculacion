using CMSVinculacion.Application.DTOs.auth;
using CMSVinculacion.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMSVinculacion.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service) => _service = service;

        /// <summary>Login: retorna access token (1h) + refresh token (7d).</summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Datos inválidos" });

            var result = await _service.LoginAsync(dto);

            if (!result.Exito)
                return Unauthorized(new { message = result.Mensaje });

            return Ok(new
            {
                token = result.AccessToken,
                refreshToken = result.RefreshToken,
                expiration = result.Expiration,
                user = result.User
            });
        }

        /// <summary>Renueva el access token usando el refresh token.</summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromQuery] string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return BadRequest("Refresh token requerido.");
            var result = await _service.RefreshTokenAsync(refreshToken);
            return result.Exito ? Ok(result) : Unauthorized(result);
        }

        /// <summary>Cierra sesión invalidando el refresh token.</summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst("userId")?.Value ?? "";
            await _service.LogoutAsync(userId);
            return NoContent();
        }
    }
}