using CMSVinculacion.Application.DTOs.media;
using CMSVinculacion.Application.Interfaces;
using CMSVinculacion.Domain.Entities.Contenido;
using Microsoft.AspNetCore.Http;

namespace CMSVinculacion.Application.Services
{
    public class MediaService : IMediaService
    {
        private readonly IMediaRepository _repo;
        private readonly string _uploadsPath;

        public MediaService(IMediaRepository repo, string uploadsPath)
        {
            _repo = repo;
            _uploadsPath = uploadsPath;
        }

        public async Task<MediaResponseDto> UploadAsync(IFormFile file, int? articleId, string uploadedBy)
        {
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType))
                throw new InvalidOperationException("Tipo de archivo no permitido. Use jpg, png o webp.");

            Directory.CreateDirectory(_uploadsPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(_uploadsPath, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var media = new MediaFiles
            {
                FileName = file.FileName,
                FilePath = $"/uploads/{fileName}",
                MimeType = file.ContentType,
                SizeBytes = file.Length,
                IsWebP = file.ContentType == "image/webp",
                UploadedAt = DateTime.UtcNow,
                ArticleId = articleId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = uploadedBy
            };

            var created = await _repo.CreateAsync(media);
            return ToDto(created);
        }

        public async Task<IEnumerable<MediaResponseDto>> GetAllAsync(int page, int pageSize) =>
            (await _repo.GetAllAsync(page, pageSize)).Select(ToDto);

        public async Task<(byte[] Data, string ContentType)?> GetThumbnailAsync(int id)
        {
            var media = await _repo.GetByIdAsync(id);
            if (media is null) return null;

            var fullPath = Path.Combine(_uploadsPath, Path.GetFileName(media.FilePath));
            if (!File.Exists(fullPath)) return null;

            var bytes = await File.ReadAllBytesAsync(fullPath);
            return (bytes, media.MimeType ?? "image/jpeg");
        }

        private static MediaResponseDto ToDto(MediaFiles m) => new()
        {
            MediaId = m.MediaId,
            FileName = m.FileName,
            FilePath = m.FilePath,
            MimeType = m.MimeType,
            SizeBytes = m.SizeBytes,
            IsWebP = m.IsWebP,
            UploadedAt = m.UploadedAt,
            ArticleId = m.ArticleId
        };
    }
}