using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.ContentDTOs
{
    public class ContentGroupDetailDto
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public int SiteId { get; set; }
        
        public string SiteName { get; set; } = string.Empty;
        
        public int? ParentId { get; set; }
        
        public string? ParentName { get; set; }
        
        public int IsDeleted { get; set; }
        
        public DateTime? CreatedDate { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        public string? Routing { get; set; }
        
        public List<ContentPageListDto> Contents { get; set; } = new List<ContentPageListDto>();
        
        public List<ContentGroupListDto> Subgroups { get; set; } = new List<ContentGroupListDto>();
    }
} 