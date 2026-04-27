namespace CMSVinculacion.Application.DTOs.media
{
    public class MediaResponseDto
    {
        public int MediaId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string? MimeType { get; set; }
        public long? SizeBytes { get; set; }
        public bool? IsWebP { get; set; }
        public DateTime UploadedAt { get; set; }
        public int? ArticleId { get; set; }
    }
}