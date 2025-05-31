using System;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.UploadDTOs
{
    /// Upload uygulama ayarlarını tutan DTO
    public class UploadAppDto
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(10)]
        public string Key { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Path { get; set; } = string.Empty;

        public DateTime? CreatedDate { get; set; }

        public int? CreatedUser { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedUser { get; set; }

        public int IsDeleted { get; set; } = 0;

        [StringLength(50)]
        public string? Password { get; set; }

        public int MaxWidth { get; set; } = 1920;

        public int MaxHeight { get; set; } = 1080;

        public int ThumbnailSize { get; set; } = 150;

        [StringLength(100)]
        public string? FilePath { get; set; }
    }
} 