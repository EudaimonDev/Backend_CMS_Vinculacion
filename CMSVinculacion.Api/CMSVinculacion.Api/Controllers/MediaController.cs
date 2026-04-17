using CMSVinculacion.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMSVinculacion.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _service;

        public MediaController(IMediaService service) => _service = service;

        /// <summary>Subida de imagen (multipart/form-data).</summary>
        [HttpPost("admin/upload")]
        [Authorize(Policy = "AdminOrEditor")]
        public async Task<IActionResult> Upload(IFormFile file, [FromQuery] int? articleId)
        {
            if (file is null || file.Length == 0)
                return BadRequest("Archivo requerido.");

            var uploadedBy = User.FindFirst("email")?.Value ?? "unknown";
            var result = await _service.UploadAsync(file, articleId, uploadedBy);
            return Ok(result);
        }

        /// <summary>Listado de medios subidos.</summary>
        [HttpGet("admin")]
        [Authorize(Policy = "AdminOrEditor")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20) =>
            Ok(await _service.GetAllAsync(page, pageSize));

        /// <summary>Thumbnail WebP comprimido de una imagen.</summary>
        [HttpGet("{id:int}/thumbnail")]
        public async Task<IActionResult> GetThumbnail(int id)
        {
            var result = await _service.GetThumbnailAsync(id);
            if (result is null) return NotFound();
            return File(result.Value.Data, result.Value.ContentType);
        }
    }
}