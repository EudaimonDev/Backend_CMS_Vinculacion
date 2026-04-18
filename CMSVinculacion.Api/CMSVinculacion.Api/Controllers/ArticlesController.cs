using CMSVinculacion.Application.DTOs.articles;
using CMSVinculacion.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            var authorId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var created = await _service.CreateAsync(dto, authorId);
            return CreatedAtAction(nameof(GetAdminById), new { id = created.ArticleId }, created);
        }

        /// <summary>Editar artículo completo.</summary>
        [HttpPut("admin/{id:int}")]
        [Authorize(Policy = "AdminOrEditor")]
        public async Task<IActionResult> Update(int id, [FromBody] ArticleUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updatedBy = User.FindFirst("email")?.Value ?? "unknown";
            var result = await _service.UpdateAsync(id, dto, updatedBy);
            return result is null ? NotFound() : Ok(result);
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
    }
}