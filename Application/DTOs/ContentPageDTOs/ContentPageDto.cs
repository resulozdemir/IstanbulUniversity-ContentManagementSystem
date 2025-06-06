using System;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.ContentPageDTOs
{
    /// Content sayfası oluşturma ve güncelleme işlemleri için kullanılan DTO
    public class ContentPageDto
    {
        public int? Id { get; set; }  // Create için null olabilir

        [Required]
        public int GroupId { get; set; }  // Content group ID'si

        public int? SiteId { get; set; }  // Site ID'si (nullable)

        [Required]
        [StringLength(200)]
        public string Header { get; set; } = string.Empty;  // Content başlığı

        public string? Content { get; set; }  // Content içeriği

        public int? OrderBy { get; set; }  // Sıralama değeri - ORDERBY alanı

        [StringLength(500)]
        public string? Link { get; set; }  // Bağlantı URL'si

        [StringLength(20)]
        public string? Column1 { get; set; }  // Ek veri alanı 1

        [StringLength(20)]
        public string? Column8 { get; set; }  // Ek veri alanı 8

        public string? ContentDev { get; set; }  // Geliştirme içeriği

        public string? ContentInner { get; set; }  // İç içerik

        public DateTime? CreatedDate { get; set; }
        public int? CreatedUser { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedUser { get; set; }
        public int IsDeleted { get; set; } = 0;
    }
} 