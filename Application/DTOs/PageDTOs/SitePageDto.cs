using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs
{
    public class SitePageDto //sayfaların yapısal bilgilerini taşır html, css, javascript vs.
    {
        public int? Id { get; set; }  // Create için null olabilir

        [Required]
        public int SiteId { get; set; }  // Site ID'si

        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }  // Sayfa adı

        public int? TemplateId { get; set; }  // Şablon ID'si

        public bool IsDefault { get; set; } = false;  // Varsayılan sayfa mı?

        public string? Html { get; set; }  // HTML içeriği

        public string? HtmlDev { get; set; }  // Geliştirme ortamı HTML içeriği

        public string? Style { get; set; }  // CSS stilleri

        public string? StyleDev { get; set; }  // Geliştirme ortamı CSS stilleri

        public string? Javascript { get; set; }  // Javascript kodu

        public string? JavascriptDev { get; set; }  // Geliştirme ortamı Javascript

        [MaxLength(50)]
        public string? Routing { get; set; }  // Yönlendirme yolu

        public bool VirtualPage { get; set; } = false;  // Sanal sayfa mı?

        public bool ReadOnly { get; set; } = false;  // Salt okunur mu?

        public bool? IsDeleted { get; set; } = false;  // Varsayılan olarak false
    }
} 