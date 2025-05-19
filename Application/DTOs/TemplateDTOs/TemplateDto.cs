using System;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.TemplateDTOs
{
    // Site şablonu (template) oluşturma, güncelleme ve yanıt işlemleri için kullanılan DTO.
    public class TemplateDto
    {
        // Yanıt olarak döndürüldüğünde veya güncelleme path'inden alındığında kullanılır.
        // Oluşturma isteğinde bu alanın değeri dikkate alınmaz veya 0 olmalıdır.
        public int Id { get; set; }

        // Şablonun adı.
        [Required]
        [StringLength(255)]
        public string? Name { get; set; }

        // Şablonla ilişkilendirilebilecek alan adı.
        // Şablonun varsayılan tema ID'si.
        [Range(1, int.MaxValue)]
        public int ThemeId { get; set; }

        // Şablonun varsayılan temasının adı (sadece yanıtlarda doldurulur).
        public string? ThemeName { get; set; }

        // Şablonun varsayılan dili.
        [StringLength(10)]
        public string? Language { get; set; }

        // Şablonun oluşturulma tarihi (sadece yanıtlarda doldurulur).
        public DateTime? CreatedDate { get; set; }

        // Şablonun son güncellenme tarihi (sadece yanıtlarda doldurulur).
        public DateTime? ModifiedDate { get; set; }
    }
} 