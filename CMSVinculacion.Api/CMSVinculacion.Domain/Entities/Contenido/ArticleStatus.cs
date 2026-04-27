using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSVinculacion.Domain.Entities.Contenido
{
    [Table(nameof(ArticleStatus), Schema = "CON")]
    public class ArticleStatus
    {
        [Key]
        public int StatusId { get; set; }
        [MaxLength(50)]
        public string StatusName { get; set; } = string.Empty;

        public ICollection<Articles>? Articles { get; set; }
    }
}
