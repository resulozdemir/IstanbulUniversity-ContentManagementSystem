using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs
{
    public class MenuDto //menu ogelerini tasır. Menu adı, baglantısı, hangi siteye ait olduğu.
    {
        public int? Id { get; set; }  // Create için null olabilir

        [Required]
        public int SiteId { get; set; }  // Site ID'si

        public int? ParentId { get; set; }  // Üst menü ID'si (null ise kök menü)

        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }  // Menü adı

        [MaxLength(500)]
        public string? Link { get; set; }  // Bağlantı URL'si

        [MaxLength(100)]
        public string? Icon { get; set; }  // İkon

        public int Openinnewtab { get; set; } = 0;  // Yeni sekmede açılsın mı?

        public int? OrderBy { get; set; }  // Sıralama değeri

        public int Isvisible { get; set; } = 1;  // Görünür mü?

        public int Isdeleted { get; set; } = 0;  // Varsayılan olarak 0
    }
} 