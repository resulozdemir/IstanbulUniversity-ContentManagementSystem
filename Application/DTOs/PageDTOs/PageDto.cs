using System;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.PageDTOs
{
    public class PageDto
    {
        public int? Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        
        public string? Html { get; set; }
        
        [Required]
        public int SiteId { get; set; }
        
        public int? ParentId { get; set; }
        
        [MaxLength(50)]
        public string? Routing { get; set; }
        
        [MaxLength(200)]
        public string? MetaTitle { get; set; }
        
        [MaxLength(500)]
        public string? MetaDescription { get; set; }
        
        [MaxLength(500)]
        public string? MetaKeywords { get; set; }
        
        public int IsDeleted { get; set; } = 0;
        
        public bool ShowInMenu { get; set; } = false;
        
        public int? MenuOrder { get; set; }
        
        [MaxLength(100)]
        public string? Layout { get; set; }
        
        public string? Style { get; set; }
        
        public string? Javascript { get; set; }
    }
} 