using System;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.NoticeDTOs
{   
    /// Duyuru oluşturma, güncelleme ve detay bilgilerini taşıyan DTO. 
    public class NoticeDto
    {
        public int Id { get; set; } // Güncelleme ve getirme için
        public string Header { get; set; } = null!;

        public string? Content { get; set; }

        public string? ContentInner { get; set; } // İçerik Detayı

        public string? Link { get; set; } 

        public string? Img { get; set; }

        public string? Tag { get; set; } // Virgülle ayrılmış etiketler

        public string? Gallery { get; set; } // Galeri bilgisi (örn: ID listesi)

        public int SiteId { get; set; }

        public int IsPublic { get; set; } = 1; // Varsayılan olarak herkese açık 

        public int? PriorityOrder { get; set; } // Öncelik sırası

        public int IsPublish { get; set; } = 1; // Varsayılan olarak yayında

        public int CategoryId { get; set; } // Kategori ID'si (TAppNotice'da var)

        [StringLength(100)]
        public string? Transaction { get; set; } // İşlem bilgisi

        public int IsBlank { get; set; } = 0; // Yeni sekmede açılıp açılmayacağı (0: hayır, 1: evet)
    }
} 