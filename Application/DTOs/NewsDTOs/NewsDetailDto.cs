using System;
using System.Collections.Generic;

namespace new_cms.Application.DTOs.NewsDTOs
{
    /// Haber detay bilgilerini içeren DTO
    public class NewsDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public int SiteId { get; set; }
        public string SiteName { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string SeoUrl { get; set; } = string.Empty;
        public string Keywords { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime PublishDate { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public string Source { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
        public List<NewsListDto> RelatedNews { get; set; } = new List<NewsListDto>();
    }
} 