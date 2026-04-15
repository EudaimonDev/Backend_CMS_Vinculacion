using CMSVinculacion.Application.DTOs.categories;
using CMSVinculacion.Application.Interfaces;
using CMSVinculacion.Domain.Entities.Catalogos;

namespace CMSVinculacion.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo) => _repo = repo;

        public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync(bool onlyPublic = false) =>
            (await _repo.GetAllAsync(onlyPublic)).Select(ToDto);

        public async Task<CategoryResponseDto?> GetByIdAsync(int id)
        {
            var cat = await _repo.GetByIdAsync(id);
            return cat is null ? null : ToDto(cat);
        }

        public async Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto)
        {
            var slug = dto.Slug ?? GenerateSlug(dto.Name);
            if (await _repo.SlugExistsAsync(slug))
                throw new InvalidOperationException($"El slug '{slug}' ya existe.");

            var category = new Categories
            {
                Name = dto.Name,
                Slug = slug,
                Description = dto.Description,
                IsPublicVisible = dto.IsPublicVisible,
                CreatedAt = DateTime.UtcNow
            };
            var created = await _repo.CreateAsync(category);
            return ToDto(created);
        }

        public async Task<CategoryResponseDto?> UpdateAsync(int id, CategoryUpdateDto dto)
        {
            var cat = await _repo.GetByIdAsync(id);
            if (cat is null) return null;

            var slug = dto.Slug ?? GenerateSlug(dto.Name);
            if (await _repo.SlugExistsAsync(slug, id))
                throw new InvalidOperationException($"El slug '{slug}' ya existe.");

            cat.Name = dto.Name;
            cat.Slug = slug;
            cat.Description = dto.Description;
            cat.IsPublicVisible = dto.IsPublicVisible;
            cat.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(cat);
            return ToDto(cat);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (await _repo.HasArticlesAsync(id))
                throw new InvalidOperationException("No se puede eliminar una categoría con artículos asociados.");

            var cat = await _repo.GetByIdAsync(id);
            if (cat is null) return false;
            await _repo.DeleteAsync(id);
            return true;
        }

        private static string GenerateSlug(string name) =>
            name.ToLower().Replace(" ", "-")
                .Replace("á", "a").Replace("é", "e").Replace("í", "i")
                .Replace("ó", "o").Replace("ú", "u").Replace("ñ", "n");

        private static CategoryResponseDto ToDto(Categories c) => new()
        {
            CategoryId = c.CategoryId,
            Name = c.Name,
            Slug = c.Slug,
            Description = c.Description,
            IsPublicVisible = c.IsPublicVisible,
            ArticleCount = c.ArticleCategories?.Count ?? 0
        };
    }
}