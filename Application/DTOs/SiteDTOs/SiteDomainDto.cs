using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.SiteDTOs
{
    /// Site domain bilgilerini taşıyan DTO
    public class SiteDomainDto //domain bilgilerini taşır
    {
        public int? Id { get; set; }  // Create için null olabilir

        [Required]
        public int SiteId { get; set; }  // Site ID'si

        [Required]
        [MaxLength(250)]
        public required string Domain { get; set; }  // Domain adı

        [Required]
        [MaxLength(20)]
        public required string Language { get; set; } = "tr";  // Dil kodu, varsayılan "tr"

        [Required]
        [MaxLength(100)]
        public required string Key { get; set; }  // Anahtar

        [MaxLength(200)]
        public string? AnalyticId { get; set; }  // Analytics ID

        [MaxLength(200)]
        public string? GoogleSiteVerification { get; set; }  // Google doğrulama kodu

        public int IsDeleted { get; set; } = 0;  // Varsayılan olarak 0
    }
} 