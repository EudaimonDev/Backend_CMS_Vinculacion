using CMSVinculacion.Domain.Entities.Contenido;

namespace CMSVinculacion.Application.Interfaces
{
    public interface IArticleRepository
    {
        Task<Articles?> GetByIdAsync(int id, bool includeDeleted = false);
        Task<(IEnumerable<Articles> Items, int Total)> GetPublishedPagedAsync(int page, int pageSize, int? categoryId = null);
        Task<IEnumerable<Articles>> GetRecentPublishedAsync(int count = 5);
        Task<IEnumerable<Articles>> GetGalleryAsync();
        Task<IEnumerable<Articles>> GetAllAdminAsync(int? statusId, int? categoryId, DateTime? startDate,int page, int pageSize);
        Task<Articles> CreateAsync(Articles article);
        Task AddCategoriesAsync(int articleId, List<int> categoryIds);
        Task UpdateAsync(Articles article);
        Task SoftDeleteAsync(int id, string deletedBy);
        Task UpdateStatusAsync(int id, int statusId, string updatedBy);
        Task UpdateCategoriesAsync(int articleId, List<int> categoryIds);
    }
}