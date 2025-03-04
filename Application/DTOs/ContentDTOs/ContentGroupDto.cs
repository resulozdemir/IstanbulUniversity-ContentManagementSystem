using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs
{
    public class ContentGroupDto //içerik gruplarını taşır. İçerikleri kategorize etmek için kullanılır
    {
        public int? Id { get; set; }  // Create için null olabilir

        [Required]
        public int SiteId { get; set; }  // Site ID'si

        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }  // Grup adı

        [MaxLength(500)]
        public string? Description { get; set; }  // Açıklama

        public int? ParentId { get; set; }  // Üst grup ID'si

        public int? OrderBy { get; set; }  // Sıralama değeri

        public bool IsVisible { get; set; } = true;  // Görünür mü?

        public bool? IsDeleted { get; set; } = false;  // Varsayılan olarak false
    }
} 