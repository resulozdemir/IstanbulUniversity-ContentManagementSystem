using System.Collections.Generic;

namespace new_cms.Application.DTOs
{
    public class PageTreeDto //sayfa hiyerarşisi için dto
    {
        public int Id { get; set; }
        
        public required string Name { get; set; }
        
        public required string Routing { get; set; }
        
        public int Isdefault { get; set; }
        
        public int Isvirtual { get; set; }
        
        public int? TemplateId { get; set; }
        
        public int? OrderBy { get; set; }
        
        public int Isvisible { get; set; }
        
        // İç içe sayfa hiyerarşisi için
        public List<PageTreeDto> Children { get; set; } = new List<PageTreeDto>();
    }
} 