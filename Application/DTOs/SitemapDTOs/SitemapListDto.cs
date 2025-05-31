using System;

namespace new_cms.Application.DTOs.SitemapDTOs
{
    /// Site haritası liste görünümü için DTO
    public class SitemapListDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? Domain { get; set; }
        public string? Lang { get; set; }
        public int? SiteId { get; set; }
        public int? ItemId { get; set; }
        public int? Column1 { get; set; }  // İçerik tipi
        public string? Column2 { get; set; }  // Kategori/ek bilgi
        public string? RedirectTo { get; set; }
        public int Active { get; set; }
        public int? CreatedUser { get; set; }
        public int? ModifiedUser { get; set; }
        public int IsDeleted { get; set; }
    }
} 