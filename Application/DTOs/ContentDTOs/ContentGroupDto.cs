using System;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.ContentDTOs
{
    public class ContentGroupDto //içerik gruplarını taşır. İçerikleri kategorize etmek için kullanılır
    {
        public int? Id { get; set; }  // Create için null olabilir

        [Required]
        public int SiteId { get; set; }  // Site ID'si

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;  // Grup adı

        [MaxLength(500)]
        public string? Description { get; set; }  // Açıklama

        public int? ParentId { get; set; }  // Üst grup ID'si

        public int? OrderBy { get; set; }  // Sıralama değeri

        public int Isvisible { get; set; } = 1;  // Görünür mü?

        public int IsDeleted { get; set; } = 0;  // Varsayılan olarak 0

        public bool IsActive { get; set; } = true;

        public int? SortOrder { get; set; }
    }
} 