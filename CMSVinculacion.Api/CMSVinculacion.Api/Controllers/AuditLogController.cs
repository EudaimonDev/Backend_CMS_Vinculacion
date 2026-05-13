using CMSVinculacion.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMSVinculacion.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOrEditor")]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogRepository _repo;

        public AuditLogController(IAuditLogRepository repo) => _repo = repo;

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecent([FromQuery] int count = 10)
        {
            var logs = await _repo.GetRecentAsync(count * 3); // traer más para filtrar
            var filtered = logs
                .Where(l => l.Action != "GET")  // ← filtrar GETs
                .Take(count)
                .Select(l => new
                {
                    action = l.Action,
                    entity = l.Entity,
                    detail = l.Detail,
                    createdAt = l.CreatedAt,
                    user = l.User != null ? l.User.Username : "Sistema"
                });
            return Ok(filtered);
        }
    }
}