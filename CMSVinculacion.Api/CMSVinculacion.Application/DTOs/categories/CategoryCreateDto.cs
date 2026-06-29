using CMSVinculacion.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CMSVinculacion.Application.DTOs.categories
{
    public class CategoryCreateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Description { get; set; }

        public bool IsPublicVisible { get; set; } = true;

        public EnumEstado Estado { get; set; } = EnumEstado.Borrador;

        [MaxLength(2000)]
        public string? ImageUrl { get; set; }

        public List<SubCategoryDto> SubCategories { get; set; } = new();
    }
}
