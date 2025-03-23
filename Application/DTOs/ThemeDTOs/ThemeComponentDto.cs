using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs
{
    public class ThemeComponentDto // tema bileşenlerinin şablon yapılarını saklar (ad, description, template, style, javascript, formjson)
    {
        public int? Id { get; set; }  // Create için null olabilir

        [Required]
        public int ThemeId { get; set; }  // Bağlı olduğu tema ID'si

        [Required]
        public int ComponentId { get; set; }  // Bileşen ID'si

        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }  // Bileşen adı

        public string? Description { get; set; }  // Açıklama

        public string? Template { get; set; }  // Şablon HTML içeriği

        public string? Style { get; set; }  // CSS stilleri

        public string? Javascript { get; set; }  // Javascript kodu

        public string? FormJson { get; set; }  // Form için JSON şeması

        public string? FormHtml { get; set; }  // Form için HTML şablonu

        public string? FormJs { get; set; }  // Form için Javascript

        public int Isdeleted { get; set; } = 0;  // Varsayılan olarak 0
    }
} 