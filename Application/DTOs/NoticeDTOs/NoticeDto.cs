using System;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.NoticeDTOs
{
    /// <summary>
    /// Duyuru oluşturma, güncelleme ve detay bilgilerini taşıyan DTO.
    /// </summary>
    public class NoticeDto
    {
        public int Id { get; set; } // Güncelleme ve getirme için

        [Required(ErrorMessage = "Başlık alanı gereklidir.")]
        [StringLength(500, ErrorMessage = "Başlık en fazla 500 karakter olabilir.")]
        public string Header { get; set; } = null!;

        public string? Content { get; set; }

        public string? ContentInner { get; set; } // İçerik Detayı

        [StringLength(500, ErrorMessage = "Link en fazla 500 karakter olabilir.")]
        public string? Link { get; set; }

        [Required(ErrorMessage = "Tarih alanı gereklidir.")]
        public DateTime OnDate { get; set; } = DateTime.UtcNow;

        [StringLength(200, ErrorMessage = "Resim URL'i en fazla 200 karakter olabilir.")]
        public string? Img { get; set; }

        [StringLength(2000, ErrorMessage = "Etiketler en fazla 2000 karakter olabilir.")]
        public string? Tag { get; set; } // Virgülle ayrılmış etiketler

        [StringLength(4000, ErrorMessage = "Galeri bilgisi en fazla 4000 karakter olabilir.")]
        public string? Gallery { get; set; } // Galeri bilgisi (örn: ID listesi)

        [Required(ErrorMessage = "Site ID alanı gereklidir.")]
        public int SiteId { get; set; }

        public int IsPublic { get; set; } = 1; // Varsayılan olarak herkese açık

        [StringLength(200, ErrorMessage = "Yazar en fazla 200 karakter olabilir.")]
        public string? Author { get; set; }

        public int? PriorityOrder { get; set; } // Öncelik sırası

        public int IsPublish { get; set; } = 1; // Varsayılan olarak yayında

        public int CategoryId { get; set; } // Kategori ID'si (TAppNotice'da var)

        [StringLength(100)]
        public string? Transaction { get; set; } // İşlem bilgisi

        public int IsBlank { get; set; } = 0; // Yeni sekmede açılıp açılmayacağı (0: hayır, 1: evet)
    }
} 