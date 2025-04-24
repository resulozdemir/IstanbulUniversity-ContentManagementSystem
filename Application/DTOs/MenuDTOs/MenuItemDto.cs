using System;

namespace new_cms.Application.DTOs.MenuDTOs
{
    public class MenuItemDto
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public int MenuId { get; set; }
        public int? ParentId { get; set; }
        public string ParentTitle { get; set; }
        public int? PageId { get; set; }
        public string PageTitle { get; set; }
        public bool? IsExternal { get; set; }
        public string Target { get; set; }
        public int? SortOrder { get; set; }
        public string Icon { get; set; }
        public bool? IsActive { get; set; }
    }
} 