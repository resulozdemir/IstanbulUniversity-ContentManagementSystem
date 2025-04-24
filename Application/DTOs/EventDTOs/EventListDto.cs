using System;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.EventDTOs
{
    /// Etkinlik listesi için özet bilgileri içeren DTO
    public class EventListDto
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        public string Summary { get; set; } = string.Empty;
        
        public DateTime EventDate { get; set; }
        
        public int SiteId { get; set; }
        
        public string SiteName { get; set; } = string.Empty;
        
        public int? CategoryId { get; set; }
        
        public string? CategoryName { get; set; }
        
        public string? SeoUrl { get; set; }
        
        public DateTime? CreatedDate { get; set; }
        
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public string Location { get; set; } = string.Empty;
        
        public string? City { get; set; }
        
        public bool IsActive { get; set; }
        
        public bool IsFeatured { get; set; }
        
        public string ImageUrl { get; set; } = string.Empty;
        
        public string? Organizer { get; set; }
        
        public decimal? Price { get; set; }
        
        public bool IsUpcoming { get; set; }
        
        public bool IsPast { get; set; }
    }
} 