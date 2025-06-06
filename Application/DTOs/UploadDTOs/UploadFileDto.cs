using System;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.UploadDTOs
{
    /// Yüklenen dosya bilgilerini tutan DTO
    public class UploadFileDto
    {
        public int? Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int SiteId { get; set; }

        [StringLength(200)]
        public string? FileId { get; set; }

        [StringLength(200)]
        public string? Salt { get; set; }

        [StringLength(500)]
        public string? Path { get; set; }

        [StringLength(200)]
        public string? Type { get; set; }

        public decimal? FileSize { get; set; }

        [StringLength(200)]
        public string? FileName { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedUser { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedUser { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
} 