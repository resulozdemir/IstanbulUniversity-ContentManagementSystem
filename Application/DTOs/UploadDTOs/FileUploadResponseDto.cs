namespace new_cms.Application.DTOs.UploadDTOs
{
    /// Dosya yükleme işlemi sonrası dönen yanıt DTO
    public class FileUploadResponseDto
    {
        /// Yüklenen dosyanın veritabanı ID'si
        public int FileId { get; set; }

        /// Dosya adı (sistem tarafından oluşturulan)
        public string FileName { get; set; } = string.Empty;

        /// Orijinal dosya adı
        public string OriginalFileName { get; set; } = string.Empty;

        /// Dosyanın tam yolu
        public string FilePath { get; set; } = string.Empty;

        /// Dosyanın web URL'si
        public string FileUrl { get; set; } = string.Empty;

        /// Dosya boyutu (byte)
        public long FileSize { get; set; }

        /// Dosya türü (MIME type)
        public string ContentType { get; set; } = string.Empty;

        /// Thumbnail URL'si (eğer oluşturulduysa)
        public string? ThumbnailUrl { get; set; }

        /// Upload başarılı mı?
        public bool Success { get; set; } = true;

        /// Hata mesajı (varsa)
        public string? ErrorMessage { get; set; }
    }
} 