using System;
using System.Collections.Generic;

namespace new_cms.Application.DTOs
{
    public class SiteDetailDto //sitenin kapsamlı bilgilerini taşır. Sayfalar, bileşenler komponentleri vs.
    {
        public int Id { get; set; }
        
        public required string Name { get; set; }
        
        public required string Domain { get; set; }
        
        public int TemplateId { get; set; }
        
        public required string TemplateName { get; set; }
        
        public int ThemeId { get; set; }
        
        public required string ThemeName { get; set; }
        
        public required string Language { get; set; }
        
        public required string AnalyticId { get; set; }
        
        public required string GoogleSiteVerification { get; set; }
        
        public int Istemplate { get; set; }
        
        public int Ispublish { get; set; }
        
        public DateTime? CreatedDate { get; set; }
        
        public required string CreatedUserName { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        public required string ModifiedUserName { get; set; }
        
        // Bağlantılı veriler
        public List<SiteDomainDto> Domains { get; set; } = new List<SiteDomainDto>();
        
        public List<SitePageDto> Pages { get; set; } = new List<SitePageDto>();
        
        public List<SiteComponentDataDto> Components { get; set; } = new List<SiteComponentDataDto>();
        
        // Component template bilgileri
        public List<ThemeComponentDto> AvailableComponents { get; set; } = new List<ThemeComponentDto>();
    }
} 