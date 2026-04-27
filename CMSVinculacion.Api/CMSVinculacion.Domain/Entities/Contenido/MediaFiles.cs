using CMSVinculacion.Domain.Entities.Seguridad;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSVinculacion.Domain.Entities.Contenido
{
    [Table(nameof(MediaFiles), Schema = "CON")]
    public class MediaFiles : Audit
    {
        [Key]
        public int MediaId { get; set; }
        
        public int? ArticleId { get; set; }
        [MaxLength(300)]
        public string FileName { get; set; } = string.Empty;
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;
        [MaxLength(100)]
        public string? MimeType { get; set; } = string.Empty;

        public long? SizeBytes { get; set; }

        public bool? IsWebP {  get; set; }

        public DateTime UploadedAt { get; set; }

        public Articles? Article { get; set; }

    }
}
