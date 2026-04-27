using CMSVinculacion.Domain.Entities.Catalogos;

namespace CMSVinculacion.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Categories>> GetAllAsync(bool onlyPublic = false);
        Task<Categories?> GetByIdAsync(int id);
        Task<Categories?> GetBySlugAsync(string slug);
        Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
        Task<bool> HasArticlesAsync(int id);
        Task<Categories> CreateAsync(Categories category);
        Task UpdateAsync(Categories category);
        Task DeleteAsync(int id);
    }
}