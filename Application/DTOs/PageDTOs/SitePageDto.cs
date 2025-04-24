using System;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.PageDTOs
{
    /// Site sayfa bilgilerini taşıyan DTO
    public class SitePageDto //sayfaların yapısal bilgilerini taşır html, css, javascript vs.
    {
        public int? Id { get; set; }  // Create için null olabilir

        [Required]
        public int SiteId { get; set; }  // Site ID'si

        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }  // Sayfa adı

        public int? TemplateId { get; set; }  // Şablon ID'si

        public int Isdefault { get; set; } = 0;  // Varsayılan sayfa mı?

        public string? Html { get; set; }  // HTML içeriği

        public string? HtmlDev { get; set; }  // Geliştirme ortamı HTML içeriği

        public string? Style { get; set; }  // CSS stilleri

        public string? StyleDev { get; set; }  // Geliştirme ortamı CSS stilleri

        public string? Javascript { get; set; }  // Javascript kodu

        public string? JavascriptDev { get; set; }  // Geliştirme ortamı Javascript

        [MaxLength(50)]
        public string? Routing { get; set; }  // Yönlendirme yolu

        public int Virtualpage { get; set; } = 0;  // Sanal sayfa mı?

        public int Readonly { get; set; } = 0;  // Salt okunur mu?

        public int IsDeleted { get; set; } = 0;  // Varsayılan olarak 0

        public string Url { get; set; } = string.Empty;
        public string Template { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Keywords { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
} 