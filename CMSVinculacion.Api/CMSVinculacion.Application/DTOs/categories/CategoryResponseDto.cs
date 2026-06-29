using CMSVinculacion.Domain.Enums;

namespace CMSVinculacion.Application.DTOs.categories
{
    public class CategoryResponseDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsPublicVisible { get; set; }
        public EnumEstado Estado { get; set; }
        public bool IsActive { get; set; }
        public int ArticleCount { get; set; }
        public string? ImageUrl { get; set; }
        public List<SubCategoryResponseDto> SubCategories { get; set; } = new();
    }

    public class SubCategoryResponseDto
    {
        public int SubCategoryId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public EnumEstado Estado { get; set; }
        public bool IsActive { get; set; }
    }
}
