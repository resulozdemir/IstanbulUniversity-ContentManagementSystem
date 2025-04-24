using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.ContentDTOs
{
    /// İçerik sayfası oluşturma ve güncelleme işlemleri için kullanılan DTO
    public class ContentPageDto
    {
        public int? Id { get; set; }  // Create için null olabilir

        [Required]
        public int SiteId { get; set; }  // Bağlı olduğu site ID'si

        [Required]
        public int GroupId { get; set; }  // İçerik grubu ID'si

        [Required]
        [StringLength(200)]
        public string Header { get; set; } = string.Empty;  // Başlık

        public string? Content { get; set; }  // Ana içerik

        public int? OrderBy { get; set; }  // Sıralama değeri

        [StringLength(500)]
        public string? Link { get; set; }  // Bağlantı URL'si

        public string? ContentDev { get; set; }  // İlave içerik alanı

        public string? ContentInner { get; set; }  // İç içerik

        public int IsDeleted { get; set; } = 0;  // Silinme durumu, 0: aktif, 1: silinmiş

        // API için ek alanlar - veritabanında olmayan
        [StringLength(500)]
        public string? Summary { get; set; }  // İçerik özeti

        [StringLength(500)]
        public string? Keywords { get; set; }  // SEO anahtar kelimeleri

        [StringLength(500)]
        public string? Description { get; set; }  // SEO açıklaması

        public DateTime? PublishDate { get; set; }  // Yayın tarihi

        [StringLength(255)]
        public string? ImageUrl { get; set; }  // Kapak görseli URL'si

        [StringLength(100)]
        public string? Author { get; set; }  // Yazar bilgisi

        public List<string> Tags { get; set; } = new List<string>();  // Etiketler
    }
} 