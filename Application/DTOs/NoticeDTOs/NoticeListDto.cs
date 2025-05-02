using System;

namespace new_cms.Application.DTOs.NoticeDTOs
{
    /// <summary>
    /// Duyuru listelemelerinde kullanılan temel bilgileri taşıyan DTO.
    /// </summary>
    public class NoticeListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty; // Header alanından map edilecek
        public DateTime PublishDate { get; set; } // OnDate alanından map edilecek
        public string? ImageUrl { get; set; } // Img alanından map edilecek
        public bool IsActive { get; set; } // IsPublish alanından map edilecek
        public int SiteId { get; set; }
        public int CategoryId { get; set; }
        // Gerekirse buraya başka özet alanlar eklenebilir
    }
} 