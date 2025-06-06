using System;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.UploadDTOs
{
    /// Dosya yükleme işlemleri için kullanılan temel DTO
    public class UploadDto
    {
        public int? Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int SiteId { get; set; }

        [Required]
        [StringLength(50)]
        public string ImageId { get; set; } = string.Empty;

        public DateTime? CreatedDate { get; set; }

        public int? CreatedUser { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedUser { get; set; }

        public int IsDeleted { get; set; } = 0;
    }
} 