using CMSVinculacion.Application.DTOs.media;
using Microsoft.AspNetCore.Http;

namespace CMSVinculacion.Application.Interfaces
{
    public interface IMediaService
    {
        Task<MediaResponseDto> UploadAsync(IFormFile file, int? articleId, string uploadedBy);
        Task<IEnumerable<MediaResponseDto>> GetAllAsync(int page, int pageSize);
        Task<(byte[] Data, string ContentType)?> GetThumbnailAsync(int id);
    }
}