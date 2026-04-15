using CMSVinculacion.Domain.Entities.Contenido;

namespace CMSVinculacion.Application.Interfaces
{
    public interface IMediaRepository
    {
        Task<MediaFiles?> GetByIdAsync(int id);
        Task<IEnumerable<MediaFiles>> GetAllAsync(int page = 1, int pageSize = 20);
        Task<MediaFiles> CreateAsync(MediaFiles media);
        Task DeleteAsync(int id);
    }
}