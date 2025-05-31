using System;

namespace new_cms.Application.DTOs.ContentPageDTOs
{
    /// Content sayfası liste görünümü için DTO
    public class ContentPageListDto
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int? SiteId { get; set; }
        public string Header { get; set; } = string.Empty;
        public string? Content { get; set; }
        public int? OrderBy { get; set; }  // Sıralama değeri - ORDERBY alanı
        public string? Link { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int IsDeleted { get; set; }
    }
} 