using System.Collections.Generic;

namespace new_cms.Application.DTOs
{
    public class PageTreeDto //sayfa hiyerarşisi için dto
    {
        public int Id { get; set; }
        
        public required string Name { get; set; }
        
        public required string Routing { get; set; }
        
        public bool IsDefault { get; set; }
        
        public bool IsVirtual { get; set; }
        
        public int? TemplateId { get; set; }
        
        public int? OrderBy { get; set; }
        
        public bool IsVisible { get; set; }
        
        // İç içe sayfa hiyerarşisi için
        public List<PageTreeDto> Children { get; set; } = new List<PageTreeDto>();
    }
} 