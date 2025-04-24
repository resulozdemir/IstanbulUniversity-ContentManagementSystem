using System;

namespace new_cms.Application.DTOs.MenuDTOs
{
    /// Menü listesi görüntüleme için DTO sınıfı
    public class MenuListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        public int SiteId { get; set; }
        public string SiteName { get; set; }
        public bool IsActive { get; set; }
        public string MenuType { get; set; }
        public string Culture { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int ItemCount { get; set; }
        public int? ParentId { get; set; }
        public string ParentName { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public int? MenuOrder { get; set; }
        public string Icon { get; set; } = string.Empty;
        public int? Status { get; set; }
    }
} 