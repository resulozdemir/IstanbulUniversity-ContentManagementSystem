using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.PageDTOs
{
    public class PageDetailDto
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string? Html { get; set; }
        
        public int SiteId { get; set; }
        
        public string SiteName { get; set; } = string.Empty;
        
        public int? ParentId { get; set; }
        
        public string? ParentName { get; set; }
        
        public string? Routing { get; set; }
        
        public string? MetaTitle { get; set; }
        
        public string? MetaDescription { get; set; }
        
        public string? MetaKeywords { get; set; }
        
        public int IsDeleted { get; set; }
        
        public bool ShowInMenu { get; set; }
        
        public int? MenuOrder { get; set; }
        
        public string? Layout { get; set; }
        
        public string? Style { get; set; }
        
        public string? Javascript { get; set; }
        
        public DateTime? CreatedDate { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        public List<PageListDto> ChildPages { get; set; } = new List<PageListDto>();
    }
} 