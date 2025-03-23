using System;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs
{
    public class EventDto  //etkinlik bilgilerini taşır. Başlık, içerik, özet, link, resim url'si vs.
    {
        public int? Id { get; set; }  // Create için null olabilir

        [Required]
        public int SiteId { get; set; }  // Site ID'si

        [Required]
        [MaxLength(255)]
        public required string Title { get; set; }  // Etkinlik başlığı

        public string? Summary { get; set; }  // Özet

        public string? Content { get; set; }  // İçerik

        [MaxLength(255)]
        public string? ImageUrl { get; set; }  // Resim URL'si

        [MaxLength(500)]
        public string? Link { get; set; }  // Bağlantı

        public DateTime? StartDate { get; set; }  // Başlangıç tarihi

        public DateTime? EndDate { get; set; }  // Bitiş tarihi

        [MaxLength(100)]
        public string? Location { get; set; }  // Konum

        public TimeSpan? StartTime { get; set; }  // Başlangıç saati

        public TimeSpan? EndTime { get; set; }  // Bitiş saati

        public int Isfeatured { get; set; } = 0;  // Öne çıkan etkinlik mi?

        public int ViewCount { get; set; } = 0;  // Görüntüleme sayısı

        public int Ispublish { get; set; } = 1;  // Yayında mı?

        public int? OrderBy { get; set; }  // Sıralama

        public int Isdeleted { get; set; } = 0;  // Varsayılan olarak 0
    }
} 