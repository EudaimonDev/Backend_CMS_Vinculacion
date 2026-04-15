using System.ComponentModel.DataAnnotations;

namespace CMSVinculacion.Application.DTOs.categories
{
    public class CategoryUpdateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(120)]
        public string? Slug { get; set; }
        [MaxLength(300)]
        public string? Description { get; set; }
        public bool IsPublicVisible { get; set; } = true;
    }
}