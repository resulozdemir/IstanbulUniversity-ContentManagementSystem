using System;

namespace new_cms.Application.DTOs.NewsDTOs
{
    public class NewsListDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public int SiteId { get; set; }
        public string SiteName { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string SeoUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public string ImageUrl { get; set; }
        public string Author { get; set; }
        public int ViewCount { get; set; }
        public string Source { get; set; }
    }
} 