using CMSVinculacion.Application.DTOs.articles;

namespace CMSVinculacion.Application.Interfaces
{
    public interface IArticleService
    {
        Task<(IEnumerable<ArticleListDto> Items, int Total)> GetPublishedAsync(int page, int pageSize, int? categoryId = null);
        Task<ArticleResponseDto?> GetPublishedByIdAsync(int id);
        Task<IEnumerable<ArticleListDto>> GetRecentAsync(int count = 5);
        Task<IEnumerable<ArticleListDto>> GetGalleryAsync();
        Task<IEnumerable<ArticleListDto>> GetAllAdminAsync(int? statusId, int? categoryId, int page, int pageSize);
        Task<ArticleResponseDto?> GetAdminByIdAsync(int id);
        Task<ArticleResponseDto> CreateAsync(ArticleCreateDto dto, int authorId);
        Task<ArticleResponseDto?> UpdateAsync(int id, ArticleUpdateDto dto, string updatedBy);
        Task<bool> DeleteAsync(int id, string deletedBy);
        Task<bool> UpdateStatusAsync(int id, ArticleStatusUpdateDto dto, string updatedBy);
    }
}