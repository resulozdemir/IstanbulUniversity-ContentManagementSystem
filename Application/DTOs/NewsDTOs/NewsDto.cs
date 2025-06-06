using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.NewsDTOs
{
    /// Haber oluşturma ve güncelleme işlemleri için kullanılan DTO
    public class NewsDto
    {
        public int? Id { get; set; }  // Create için null olabilir
        
        [Required]
        [StringLength(500)]
        public string Header { get; set; } = string.Empty;  // Başlık
        
        public string? Content { get; set; }  // Ana içerik
        
        [StringLength(500)]
        public string? Link { get; set; }  // Bağlantı URL'si 
        
        [StringLength(4000)]
        public string? Img { get; set; }  // Resim URL'si
        
        [StringLength(2000)]
        public string? Tag { get; set; }  // Etiketler
        
        public int InSlider { get; set; } = 0;  // Slayt gösterisinde görüntülensin mi?
        
        [Required]
        public int SiteId { get; set; }  // Bağlı olduğu site ID'si
        
        public int IsPublic { get; set; } = 1;  // Halka açık mı? 
        
        public string? ContentInner { get; set; }  // İç içerik
        
        public int? PriorityOrder { get; set; }  // Öncelik sırası
        
        public int IsPublish { get; set; } = 0;  // Yayımlandı mı?
        
        public int IsDeleted { get; set; } = 0;  // Silinme durumu
    }
} 