using System.ComponentModel.DataAnnotations.Schema;
using CMSVinculacion.Domain.Entities.Catalogos;

namespace CMSVinculacion.Domain.Entities.Contenido
{
    [Table(nameof(ArticleCategory), Schema = "CON")]
    public class ArticleCategory
    {
        public int ArticleId { get; set; }
        public Articles? Article { get; set; }

        public int CategoryId { get; set; }
        public Categories? Category { get; set; }
    }
}