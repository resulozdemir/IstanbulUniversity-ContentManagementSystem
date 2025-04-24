using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.EventDTOs
{
    /// Etkinlik detay bilgilerini içeren DTO
    public class EventDetailDto
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        public string Content { get; set; } = string.Empty;
        
        public string Summary { get; set; } = string.Empty;
        
        public int SiteId { get; set; }
        
        public string SiteName { get; set; } = string.Empty;
        
        public int? CategoryId { get; set; }
        
        public string? CategoryName { get; set; }
        
        public string SeoUrl { get; set; } = string.Empty;
        
        public string? Keywords { get; set; }
        
        public string? Description { get; set; }
        
        public bool IsActive { get; set; }
        
        public bool IsFeatured { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public string Location { get; set; } = string.Empty;
        
        public string? City { get; set; }
        
        public string Address { get; set; } = string.Empty;
        
        public string ImageUrl { get; set; } = string.Empty;
        
        public string? Organizer { get; set; }
        
        public string? ContactInfo { get; set; }
        
        public decimal? Price { get; set; }
        
        public string? TicketUrl { get; set; }
        
        public int ViewCount { get; set; }
        
        public bool IsUpcoming { get; set; }
        
        public bool IsPast { get; set; }
        
        public List<string> Tags { get; set; } = new List<string>();
        
        public List<EventListDto> RelatedEvents { get; set; } = new List<EventListDto>();
        
        public DateTime EventDate { get; set; }
    }
} 