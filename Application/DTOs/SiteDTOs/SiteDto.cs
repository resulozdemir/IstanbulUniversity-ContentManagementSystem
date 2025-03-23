using System;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs
{
    public class SiteDto //site bilgileri
    {
        public int? Id { get; set; }  // Hem olusturma hem guncelleme için kullanilir, create için null olabilir.

        [Required]
        [MaxLength(255)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Domain { get; set; }

        [Required]
        public int TemplateId { get; set; }  //site şablonu

        [Required]
        public int ThemeId { get; set; } //renkler fontlar vs.

        [MaxLength(10)]
        public required string Language { get; set; } = "tr";  

        public string? AnalyticId { get; set; }  

        public int? PbysId { get; set; }  
        
        [MaxLength(50)]
        public string? Key { get; set; } 
        
        [MaxLength(400)]
        public string? WpAddress { get; set; }  
        
        public string? GoogleSiteVerification { get; set; }  

        public int Ispublish { get; set; } = 0; 
        
        public DateTime? CreatedDate { get; set; }
        
        public int? CreatedUser { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        public int? ModifiedUser { get; set; }

        public int Isdeleted { get; set; } = 0; 
    }
}
