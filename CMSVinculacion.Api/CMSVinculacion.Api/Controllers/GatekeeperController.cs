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

        public GatekeeperController(IGatekeeperService service)
        {
            _service = service;
        }

        /// <summary>Registra un nuevo visitante y emite su token de acceso.</summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] GatekeeperRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var resultado = await _service.RegistrarVisitanteAsync(request, ip);

            if (!resultado.Exito)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        /// <summary>Valida si un token sigue siendo vigente.</summary>
        [HttpGet("validate")]
        public async Task<IActionResult> Validate([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Token requerido.");

            var resultado = await _service.ValidarTokenAsync(token);

            if (!resultado.Exito)
                return Unauthorized(resultado);

            return Ok(resultado);
        }
    }
}
