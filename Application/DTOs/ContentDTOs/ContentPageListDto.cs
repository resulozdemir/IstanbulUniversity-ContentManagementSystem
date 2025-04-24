using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.ContentDTOs
{
    public class ContentPageListDto
    {
        public int Id { get; set; }
        
        public string Header { get; set; } = string.Empty;
        
        public string? Summary { get; set; }
        
        public int SiteId { get; set; }
        
        public string SiteName { get; set; } = string.Empty;
        
        public int? GroupId { get; set; }
        
        public string? GroupName { get; set; }
        
        public string? SeoUrl { get; set; }
        
        public DateTime? CreatedDate { get; set; }
        
        public DateTime? PublishDate { get; set; }
        
        public int IsDeleted { get; set; }
        
        public string? ImageUrl { get; set; }
        
        public string? Author { get; set; }
        
        public int ViewCount { get; set; }
        
        public int? OrderBy { get; set; }
        
        public List<string> Tags { get; set; } = new List<string>();
    }
} 