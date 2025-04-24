using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.EventDTOs
{
    /// Etkinlik oluşturma ve güncelleme işlemleri için kullanılan DTO
    public class EventDto
    {
        public int? Id { get; set; }  // Create için null olabilir
        
        [Required]
        [StringLength(500)]
        public string Header { get; set; } = string.Empty;  // Başlık
        
        public string? Content { get; set; }  // Ana içerik
        
        [StringLength(400)]
        public string? Summary { get; set; }  // Özet
        
        [StringLength(500)]
        public string? Link { get; set; }  // Bağlantı URL'si
        
        [Required]
        public DateTime OnDate { get; set; }  // Etkinlik tarihi
        
        [StringLength(200)]
        public string? Img { get; set; }  // Resim URL'si
        
        [StringLength(2000)]
        public string? Tag { get; set; }  // Etiketler
        
        [StringLength(4000)]
        public string? Gallery { get; set; }  // Galeri resimleri
        
        [Required]
        public int SiteId { get; set; }  // Bağlı olduğu site ID'si
        
        [StringLength(300)]
        public string? Map { get; set; }  // Harita bağlantısı
        
        public int IsPublic { get; set; } = 1;  // Halka açık mı?
        
        [StringLength(200)]
        public string? Author { get; set; }  // Yazar
        
        [StringLength(200)]
        public string? Address { get; set; }  // Adres
        
        public string? ContentInner { get; set; }  // İç içerik
        
        public int? PriorityOrder { get; set; }  // Öncelik sırası
        
        public int IsPublish { get; set; } = 0;  // Yayımlandı mı?
        
        public int IsDeleted { get; set; } = 0;  // Silinme durumu
    }
} 