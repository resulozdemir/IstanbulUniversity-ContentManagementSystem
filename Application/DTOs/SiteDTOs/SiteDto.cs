using System;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.SiteDTOs
{
    /// Site oluşturma ve güncelleme işlemleri için kullanılan DTO
    public class SiteDto
    {
        public int? Id { get; set; }  // Create için null olabilir

        [Required]
        [StringLength(500)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Domain { get; set; } = string.Empty;

        [Required]
        public int? TemplateId { get; set; }  // Templatid: site şablonu

        [Required]
        public int ThemeId { get; set; }  // Themeid: renkler fontlar vs.

        [StringLength(200)]
        public string Language { get; set; } = "tr";

        [StringLength(200)]
        public string? AnalyticId { get; set; }

        public int? PbysId { get; set; }
        
        [StringLength(50)]
        public string? Key { get; set; } 
        
        [StringLength(200)]
        public string? GoogleSiteVerification { get; set; }

        public int IsPublish { get; set; }

        public int IsDeleted { get; set; } = 0;
    }
}
