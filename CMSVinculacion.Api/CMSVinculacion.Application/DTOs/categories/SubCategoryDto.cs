using CMSVinculacion.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CMSVinculacion.Application.DTOs.categories
{
    public class SubCategoryDto
    {
        public int? SubCategoryId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Description { get; set; }

        public EnumEstado Estado { get; set; } = EnumEstado.Borrador;
    }
}
