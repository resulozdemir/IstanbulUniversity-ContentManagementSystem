using System;
using System.Collections.Generic;

namespace new_cms.Application.DTOs
{
    public class SiteListDto //siteleri listelerken site hakkında daha falz bilgi saylamak için kullanılır sayfa sayısı haber sayısı gibi.
    {
        public int Id { get; set; }
        
        public required string Name { get; set; }
        
        public required string Domain { get; set; }
        
        public int TemplateId { get; set; }
        
        public required string TemplateName { get; set; }
        
        public int ThemeId { get; set; }
        
        public required string ThemeName { get; set; }
        
        public required string Language { get; set; }
        
        public int Istemplate { get; set; }
        
        public int Ispublish { get; set; }
        
        public DateTime? CreatedDate { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        // Sayfanın içerikler sayıları
        public int PageCount { get; set; }
        
        public int NewsCount { get; set; }
        
        public int EventCount { get; set; }
        
        public int ComponentCount { get; set; }
        
        // Domain bilgileri
        public List<SiteDomainDto> Domains { get; set; } = new List<SiteDomainDto>();
    }
} 