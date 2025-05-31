using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.UploadDTOs
{
    /// Dosya yükleme isteği için kullanılan DTO
    public class FileUploadRequestDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;

        [Required]
        public int SiteId { get; set; }

        [Required]
        public int UserId { get; set; }

        /// Dosya kategorisi (images, documents vb.)
        [StringLength(50)]
        public string Category { get; set; } = "images";

        /// Alt klasör adı
        [StringLength(100)]
        public string? SubFolder { get; set; }

        /// Thumbnail oluşturulsun mu?
        public bool CreateThumbnail { get; set; } = true;

        /// Maksimum genişlik (resim dosyaları için)
        public int? MaxWidth { get; set; }

        /// Maksimum yükseklik (resim dosyaları için)  
        public int? MaxHeight { get; set; }
    }
} 