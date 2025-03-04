using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs
{
    public class ThemeDto // tema bilgileri (ad, header, footer, silindi mi)
    {
        public int? Id { get; set; }  // Create için null olabilir

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }  // Tema adı

        public string? Header { get; set; }  // Header şablonu

        public string? Footer { get; set; }  // Footer şablonu

        public bool? IsDeleted { get; set; } = false;  // Varsayılan olarak false
    }
} 