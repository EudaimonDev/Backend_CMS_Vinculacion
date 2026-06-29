using CMSVinculacion.Application.DTOs.categories;
using CMSVinculacion.Application.Interfaces;
using CMSVinculacion.Domain.Entities.Catalogos;
using CMSVinculacion.Domain.Enums;

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
            var cat = await _repo.GetByIdAsync(id, includeInactive: true);
            return cat is null ? null : ToDto(cat);
        }

        public async Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto)
        {
            var slug = await ResolveUniqueCategorySlugAsync(GenerateSlug(dto.Name));

            var category = new Categories
            {
                Name = dto.Name.Trim(),
                Slug = slug,
                Description = dto.Description?.Trim(),
                IsPublicVisible = dto.IsPublicVisible,
                ImageUrl = dto.ImageUrl?.Trim(),
                Estado = dto.Estado,
                CreatedAt = DateTime.UtcNow,
                SubCategories = new List<SubCategories>()
            };

            ApplyEstado(category);

            foreach (var subDto in dto.SubCategories.Where(s => !string.IsNullOrWhiteSpace(s.Name)))
            {
                var baseSlug = GenerateSlug(subDto.Name);
                var subSlug = baseSlug;
                var counter = 1;
                while (category.SubCategories!.Any(s => s.Slug == subSlug))
                {
                    subSlug = $"{baseSlug}-{counter++}";
                }

                var sub = MapNewSubCategory(subDto, subSlug);
                category.SubCategories!.Add(sub);
            }

            if (category.Estado == EnumEstado.Archivado && category.SubCategories != null)
            {
                foreach (var sub in category.SubCategories)
                {
                    sub.Estado = EnumEstado.Archivado;
                    sub.IsActive = false;
                }
            }

            var created = await _repo.CreateAsync(category);
            return ToDto(created);
        }

        public async Task<CategoryResponseDto?> UpdateAsync(int id, CategoryUpdateDto dto)
        {
            var cat = await _repo.GetByIdAsync(id, includeInactive: true);
            if (cat is null) return null;

            var slug = await ResolveUniqueCategorySlugAsync(GenerateSlug(dto.Name), id);

            cat.Name = dto.Name.Trim();
            cat.Slug = slug;
            cat.Description = dto.Description?.Trim();
            cat.IsPublicVisible = dto.IsPublicVisible;
            cat.ImageUrl = dto.ImageUrl?.Trim();
            cat.Estado = dto.Estado;
            cat.UpdatedAt = DateTime.UtcNow;

            ApplyEstado(cat);
            await SyncSubCategoriesAsync(cat, dto.SubCategories);

            if (cat.Estado == EnumEstado.Archivado && cat.SubCategories != null)
            {
                foreach (var sub in cat.SubCategories.Where(s => s.DeletedAt == null))
                {
                    sub.Estado = EnumEstado.Archivado;
                    sub.IsActive = false;
                }
            }

            await _repo.UpdateAsync(cat);
            return ToDto(cat);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (await _repo.HasArticlesAsync(id))
                throw new InvalidOperationException("No se puede eliminar una categoría con artículos asociados.");

            var cat = await _repo.GetByIdAsync(id, includeInactive: true);
            if (cat is null) return false;

            await _repo.DeleteAsync(id);
            return true;
        }

        private async Task SyncSubCategoriesAsync(Categories cat, IList<SubCategoryDto> dtos)
        {
            cat.SubCategories ??= new List<SubCategories>();
            var incoming = dtos.Where(d => !string.IsNullOrWhiteSpace(d.Name)).ToList();
            var incomingIds = incoming
                .Where(d => d.SubCategoryId.HasValue)
                .Select(d => d.SubCategoryId!.Value)
                .ToHashSet();

            foreach (var existing in cat.SubCategories.Where(s => s.DeletedAt == null && !incomingIds.Contains(s.SubCategoryId)))
            {
                existing.IsActive = false;
                existing.Estado = EnumEstado.Archivado;
                existing.DeletedAt = DateTime.UtcNow;
            }

            foreach (var subDto in incoming)
            {
                if (subDto.SubCategoryId.HasValue)
                {
                    var existing = cat.SubCategories.FirstOrDefault(s => s.SubCategoryId == subDto.SubCategoryId.Value);
                    if (existing is null || existing.DeletedAt != null) continue;

                    existing.Name = subDto.Name.Trim();
                    existing.Description = subDto.Description?.Trim();
                    existing.Estado = subDto.Estado;
                    existing.Slug = await ResolveUniqueSubCategorySlugAsync(
                        cat.CategoryId,
                        GenerateSlug(subDto.Name),
                        existing.SubCategoryId);
                    existing.UpdatedAt = DateTime.UtcNow;
                    ApplyEstado(existing);
                }
                else
                {
                    var subSlug = await ResolveUniqueSubCategorySlugAsync(cat.CategoryId, GenerateSlug(subDto.Name));
                    var sub = MapNewSubCategory(subDto, subSlug);
                    sub.CategoryId = cat.CategoryId;
                    cat.SubCategories.Add(sub);
                }
            }
        }

        private static SubCategories MapNewSubCategory(SubCategoryDto dto, string slug)
        {
            var sub = new SubCategories
            {
                Name = dto.Name.Trim(),
                Slug = slug,
                Description = dto.Description?.Trim(),
                Estado = dto.Estado,
                CreatedAt = DateTime.UtcNow
            };
            ApplyEstado(sub);
            return sub;
        }

        private static void ApplyEstado(Categories category)
        {
            category.IsActive = category.Estado != EnumEstado.Archivado;
        }

        private static void ApplyEstado(SubCategories subCategory)
        {
            subCategory.IsActive = subCategory.Estado != EnumEstado.Archivado;
        }

        private async Task<string> ResolveUniqueCategorySlugAsync(string baseSlug, int? excludeId = null)
        {
            var slug = baseSlug;
            var counter = 1;
            while (await _repo.SlugExistsAsync(slug, excludeId))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }
            return slug;
        }

        private async Task<string> ResolveUniqueSubCategorySlugAsync(int categoryId, string baseSlug, int? excludeId = null)
        {
            if (categoryId == 0)
                return baseSlug;

            var slug = baseSlug;
            var counter = 1;
            while (await _repo.SubCategorySlugExistsAsync(categoryId, slug, excludeId))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }
            return slug;
        }

        private static string GenerateSlug(string name) =>
            name.ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("á", "a").Replace("é", "e").Replace("í", "i")
                .Replace("ó", "o").Replace("ú", "u").Replace("ñ", "n")
                .Trim('-');

        private static CategoryResponseDto ToDto(Categories c) => new()
        {
            CategoryId = c.CategoryId,
            Name = c.Name,
            Slug = c.Slug,
            Description = c.Description,
            IsPublicVisible = c.IsPublicVisible,
            Estado = c.Estado,
            IsActive = c.IsActive,
            ImageUrl = c.ImageUrl,
            ArticleCount = c.ArticleCategories?.Count ?? 0,
            SubCategories = c.SubCategories?
                .Where(s => s.DeletedAt == null)
                .OrderBy(s => s.Name)
                .Select(s => new SubCategoryResponseDto
                {
                    SubCategoryId = s.SubCategoryId,
                    CategoryId = s.CategoryId,
                    Name = s.Name,
                    Slug = s.Slug,
                    Description = s.Description,
                    Estado = s.Estado,
                    IsActive = s.IsActive
                })
                .ToList() ?? new List<SubCategoryResponseDto>()
        };
    }
}
