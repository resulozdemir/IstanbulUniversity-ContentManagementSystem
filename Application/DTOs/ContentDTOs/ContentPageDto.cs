using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs
{
    public class ContentPageDto //içerik sayfalarının verilerini taşır, başlık, içerik, hangi sayfaya ait, link.
    {
        public int? Id { get; set; }  // Create için null olabilir

        [Required]
        public int SiteId { get; set; }  // Bağlı olduğu site ID'si

        [Required]
        public int GroupId { get; set; }  // İçerik grubu ID'si

        [Required]
        [MaxLength(200)]
        public required string Header { get; set; }  // Başlık

        public string? Content { get; set; }  // Ana içerik

        public string? ContentDev { get; set; }  // Geliştirme ortamı içeriği

        public string? ContentInner { get; set; }  // İç içerik

        public int? OrderBy { get; set; }  // Sıralama değeri

        [MaxLength(500)]
        public string? Link { get; set; }  // Bağlantı URL'si

        public int Isdeleted { get; set; } = 0;  // Varsayılan olarak 0
    }
} 