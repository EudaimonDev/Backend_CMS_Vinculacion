using CMSVinculacion.Application.DTOs.articles;
using CMSVinculacion.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace CMSVinculacion.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _service;

        public ArticlesController(IArticleService service) => _service = service;

        // ── VISTA PÚBLICA ────────────────────────────────────────

        /// <summary>Listado paginado de artículos publicados.</summary>
        [HttpGet]
        public async Task<IActionResult> GetPublished(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? categoryId = null)
        {
            var (items, total) = await _service.GetPublishedAsync(page, pageSize, categoryId);
            return Ok(new { items, total, page, pageSize });
        }

        /// <summary>Artículos publicados recientes.</summary>
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecent([FromQuery] int count = 5) =>
            Ok(await _service.GetRecentAsync(count));

        /// <summary>Vista galería tipo masonry.</summary>
        [HttpGet("gallery")]
        public async Task<IActionResult> GetGallery() =>
            Ok(await _service.GetGalleryAsync());

        /// <summary>Detalle de artículo publicado.</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var article = await _service.GetPublishedByIdAsync(id);
            return article is null ? NotFound() : Ok(article);
        }

        // ── BACK-OFFICE ──────────────────────────────────────────

        /// <summary>Listado admin (todos los estados).</summary>
        [HttpGet("admin")]
        [Authorize(Policy = "AdminOrEditor")]
        public async Task<IActionResult> GetAllAdmin(
            [FromQuery] int? statusId,
            [FromQuery] int? categoryId,
            [FromQuery] DateTime? startDate,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20) =>
            Ok(await _service.GetAllAdminAsync(statusId, categoryId, startDate, page, pageSize));

        /// <summary>Detalle admin (incluye borradores/publicados).</summary>
        [HttpGet("admin/{id:int}")]
        [Authorize(Policy = "AdminOrEditor")]
        public async Task<IActionResult> GetAdminById(int id)
        {
            var article = await _service.GetAdminByIdAsync(id);
            return article is null ? NotFound() : Ok(article);
        }

        /// <summary>Crear artículo.</summary>
        [HttpPost("admin")]
        [Authorize(Policy = "AdminOrEditor")]
        public async Task<IActionResult> Create([FromBody] ArticleCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var authorId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
                var created = await _service.CreateAsync(dto, authorId);
                return CreatedAtAction(nameof(GetAdminById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>Editar artículo completo.</summary>
        [HttpPut("admin/{id:int}")]
        [Authorize(Policy = "AdminOrEditor")]
        public async Task<IActionResult> Update(int id, [FromBody] ArticleUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var updatedBy = User.FindFirst("email")?.Value ?? "unknown";
                var result = await _service.UpdateAsync(id, dto, updatedBy);
                return result is null ? NotFound() : Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>Cambiar estado borrador ↔ publicado.</summary>
        [HttpPatch("admin/{id:int}/status")]
        [Authorize(Policy = "AdminOrEditor")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] ArticleStatusUpdateDto dto)
        {
            var updatedBy = User.FindFirst("email")?.Value ?? "unknown";
            var ok = await _service.UpdateStatusAsync(id, dto, updatedBy);
            return ok ? NoContent() : NotFound();
        }

        /// <summary>Eliminar artículo (soft delete).</summary>
        [HttpDelete("admin/{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            var deletedBy = User.FindFirst("email")?.Value ?? "unknown";
            var ok = await _service.DeleteAsync(id, deletedBy);
            return ok ? NoContent() : NotFound();
        }

        /// <summary>Resuelve enlaces Canva (incluye canva.link) al ID embebible.</summary>
        [HttpGet("canva/resolve")]
        [AllowAnonymous]
        public async Task<IActionResult> ResolveCanvaUrl(
            [FromQuery] string url,
            IHttpClientFactory httpClientFactory)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest(new { message = "URL requerida" });

            var trimmed = url.Trim();
            var parsed = ParseCanvaUrl(trimmed);
            if (parsed is not null)
                return Ok(BuildCanvaResolveResponse(parsed.Value.designId, parsed.Value.shareToken));

            if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri))
                return BadRequest(new { message = "URL no válida" });

            var host = uri.Host.ToLowerInvariant();
            var isCanvaHost = host is "canva.link" or "www.canva.link" || host.EndsWith("canva.com");
            if (!isCanvaHost)
                return BadRequest(new { message = "Solo se admiten enlaces de Canva" });

            try
            {
                var client = httpClientFactory.CreateClient("CanvaResolver");
                using var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
                var finalUrl = response.RequestMessage?.RequestUri?.ToString() ?? trimmed;
                parsed = ParseCanvaUrl(finalUrl);

                if (parsed is null && response.Headers.Location is not null)
                    parsed = ParseCanvaUrl(response.Headers.Location.ToString());

                if (parsed is null)
                    return BadRequest(new { message = "No se pudo obtener el ID del diseño Canva" });

                return Ok(BuildCanvaResolveResponse(parsed.Value.designId, parsed.Value.shareToken));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error al resolver enlace Canva: {ex.Message}" });
            }
        }

        private static readonly HashSet<string> CanvaPathActions = new(StringComparer.OrdinalIgnoreCase)
        {
            "view", "edit", "watch", "present", "embed"
        };

        private static (string designId, string? shareToken)? ParseCanvaUrl(string url)
        {
            var match = Regex.Match(url, @"canva\.com/design/([A-Za-z0-9_-]+)(?:/([^/?#]+))?", RegexOptions.IgnoreCase);
            if (!match.Success) return null;

            var designId = match.Groups[1].Value;
            var second = match.Groups[2].Success ? match.Groups[2].Value : null;
            string? shareToken = null;
            if (!string.IsNullOrEmpty(second) && !CanvaPathActions.Contains(second))
                shareToken = second;

            return (designId, shareToken);
        }

        private static object BuildCanvaResolveResponse(string designId, string? shareToken)
        {
            var embedUrl = shareToken is not null
                ? $"https://www.canva.com/design/{designId}/{shareToken}/view?embed"
                : $"https://www.canva.com/design/{designId}/view?embed";
            var viewUrl = shareToken is not null
                ? $"https://www.canva.com/design/{designId}/{shareToken}/view"
                : $"https://www.canva.com/design/{designId}/view";

            return new { designId, shareToken, embedUrl, viewUrl };
        }
    }
}