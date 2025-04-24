using System;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.ThemeDTOs
{
    /// Tema bileşenlerini taşıyan DTO
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
        
        public string? ComponentName { get; set; }  // Bağlı bileşenin adı (ilişkili tablodaki)

        public string? Description { get; set; }  // Açıklama

        public string? Template { get; set; }  // Şablon HTML içeriği

        public string? Style { get; set; }  // CSS stilleri

        public string? Javascript { get; set; }  // Javascript kodu

        public string? FormJson { get; set; }  // Form için JSON şeması

        public string? FormHtml { get; set; }  // Form için HTML şablonu

        public string? FormJs { get; set; }  // Form için Javascript

        public int IsDeleted { get; set; } = 0;  // Varsayılan olarak 0

        public string Settings { get; set; } = string.Empty;

        public int OrderBy { get; set; }

        public bool IsActive { get; set; } = true;
    }
} 