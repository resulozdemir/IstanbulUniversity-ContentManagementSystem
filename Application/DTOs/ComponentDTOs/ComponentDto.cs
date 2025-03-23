using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs
{
    public class ComponentDto   //bileşenin genel yapısını taşır.
    {
        public int? Id { get; set; } // Create için null olabilir

        [Required]
        [MaxLength(255)]
        public required string Name { get; set; }  // Bileşen adı

        public string? Description { get; set; }  // Açıklama (opsiyonel)

        public string? Template { get; set; }  // Şablon bilgisi (opsiyonel)

        public string? Style { get; set; }  // CSS veya özel stiller

        public string? FormJson { get; set; }  // JSON formatında form verileri

        public int Isdeleted { get; set; } = 0;  // Soft delete için
    }
}
