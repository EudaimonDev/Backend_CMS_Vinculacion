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

        public async Task<bool> DeleteAsync(int id, string deletedBy)
        {
            var media = await _repo.GetByIdAsync(id);
            if (media is null) return false;

            // Eliminar archivo físico
            var fullPath = Path.Combine(_uploadsPath, Path.GetFileName(media.FilePath));
            if (File.Exists(fullPath))
                File.Delete(fullPath);

            // Eliminar registro de BD
            await _repo.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<MediaResponseDto>> UploadSlidesAsync(IFormFile file, int? articleId, string uploadedBy)
        {
            var validExtensions = new[] { ".pptx", ".ppt" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!validExtensions.Contains(ext))
                throw new InvalidOperationException("El archivo debe ser una presentación .pptx o .ppt");

            Directory.CreateDirectory(_uploadsPath);

            var tempId = Guid.NewGuid().ToString();
            var tempDir = Path.Combine(Path.GetTempPath(), tempId);
            Directory.CreateDirectory(tempDir);

            var pptxPath = Path.Combine(tempDir, $"{tempId}{ext}");
            await using (var stream = new FileStream(pptxPath, FileMode.Create))
                await file.CopyToAsync(stream);

            try
            {
                // 1. Convertir .pptx a PDF
                await RunProcessAsync("soffice", $"--headless --convert-to pdf --outdir \"{tempDir}\" \"{pptxPath}\"");

                var pdfPath = Path.Combine(tempDir, $"{tempId}.pdf");
                if (!File.Exists(pdfPath))
                    throw new InvalidOperationException("No se pudo convertir la presentación. Verifique que LibreOffice esté instalado en el servidor.");

                // 2. Convertir cada página del PDF a JPEG
                var imagePrefix = Path.Combine(tempDir, "slide");
                await RunProcessAsync("pdftoppm", $"-jpeg -r 130 \"{pdfPath}\" \"{imagePrefix}\"");

                var slideFiles = Directory.GetFiles(tempDir, "slide-*.jpg")
                    .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (slideFiles.Count == 0)
                    throw new InvalidOperationException("La presentación no generó diapositivas.");

                var results = new List<MediaResponseDto>();
                var baseName = Path.GetFileNameWithoutExtension(file.FileName);

                for (int i = 0; i < slideFiles.Count; i++)
                {
                    var destFileName = $"{Guid.NewGuid()}.jpg";
                    var destPath = Path.Combine(_uploadsPath, destFileName);
                    File.Copy(slideFiles[i], destPath, overwrite: true);

                    var media = new MediaFiles
                    {
                        FileName = $"{baseName} - Diapositiva {i + 1}",
                        FilePath = $"/uploads/{destFileName}",
                        MimeType = "image/jpeg",
                        SizeBytes = new FileInfo(destPath).Length,
                        IsWebP = false,
                        UploadedAt = DateTime.UtcNow,
                        ArticleId = articleId,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = uploadedBy
                    };

                    var created = await _repo.CreateAsync(media);
                    results.Add(ToDto(created));
                }

                return results;
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, recursive: true);
            }
        }

        private static async Task RunProcessAsync(string command, string args)
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = command,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = System.Diagnostics.Process.Start(psi)
                ?? throw new InvalidOperationException($"No se pudo ejecutar: {command}");

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                var error = await process.StandardError.ReadToEndAsync();
                throw new InvalidOperationException($"Error en {command}: {error}");
            }
        }
    }
}