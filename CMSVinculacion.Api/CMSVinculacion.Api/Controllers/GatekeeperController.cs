using CMSVinculacion.Application.DTOs.gatekeeper;
using CMSVinculacion.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace CMSVinculacion.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GatekeeperController : ControllerBase
    {
        private readonly IGatekeeperService _service;

        public GatekeeperController(IGatekeeperService service) => _service = service;

        /// <summary>Registrar visitante público.</summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] GatekeeperRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var result = await _service.RegistrarVisitanteAsync(dto, ipAddress);

            // Si el correo ya existe retornar 409 para que el front lo maneje
            if (!result.Exito && result.Visitor is null)
                return BadRequest(new { message = result.Mensaje });

            if (!result.Exito && result.Visitor is not null)
                return Conflict(new
                {
                    message = result.Mensaje,
                    token = result.Token,
                    expiresIn = result.ExpiresIn,
                    visitor = result.Visitor
                });

            return Ok(result);
        }

        /// <summary>Validar token de visitante.</summary>
        [HttpGet("validate")]
        public async Task<IActionResult> Validate([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new { message = "Token requerido." });

            var result = await _service.ValidarTokenAsync(token);
            return result.Exito ? Ok(result) : Unauthorized(result);
        }
    }
}