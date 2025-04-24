using System;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.PageDTOs
{
    public class PageListDto
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public int SiteId { get; set; }
        
        public string SiteName { get; set; } = string.Empty;
        
        public int? ParentId { get; set; }
        
        public string? ParentName { get; set; }
        
        public string? Routing { get; set; }
        
        public DateTime? CreatedDate { get; set; }
        
        public int IsDeleted { get; set; }
        
        public bool ShowInMenu { get; set; }
        
        public int? MenuOrder { get; set; }
        
        public int ChildCount { get; set; }
    }
} 