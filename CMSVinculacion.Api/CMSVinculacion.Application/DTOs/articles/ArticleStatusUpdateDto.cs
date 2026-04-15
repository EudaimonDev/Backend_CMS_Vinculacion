using System.ComponentModel.DataAnnotations;

namespace CMSVinculacion.Application.DTOs.articles
{
    public class ArticleStatusUpdateDto
    {
        [Required]
        public int StatusId { get; set; }
    }
}