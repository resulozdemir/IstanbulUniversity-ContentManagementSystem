using System;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs
{
    public class NewsDto    //haber içeriklerini taşır. Başlık, özet, resim url'si vs.
    {
        public int? Id { get; set; }  // Create için null olabilir

        [Required]
        public int SiteId { get; set; }  // Site ID'si

        [Required]
        [MaxLength(255)]
        public required string Title { get; set; }  // Haber başlığı

        public string? Summary { get; set; }  // Özet

        public string? Content { get; set; }  // İçerik

        [MaxLength(255)]
        public string? ImageUrl { get; set; }  // Resim URL'si

        [MaxLength(500)]
        public string? Link { get; set; }  // Bağlantı

        public DateTime? PublishDate { get; set; }  // Yayın tarihi

        public DateTime? EndDate { get; set; }  // Bitiş tarihi

        public int Isfeatured { get; set; } = 0;  // Öne çıkan haber mi?

        public int ViewCount { get; set; } = 0;  // Görüntüleme sayısı

        public int Ispublish { get; set; } = 1;  // Yayında mı?

        public int? OrderBy { get; set; }  // Sıralama

        public int Isdeleted { get; set; } = 0;  // Varsayılan olarak 0
    }
} 