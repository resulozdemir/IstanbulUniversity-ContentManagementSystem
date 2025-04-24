using System;
using System.Collections.Generic;

namespace new_cms.Application.DTOs.MenuDTOs
{
    public class MenuDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SiteId { get; set; }
        public string SiteName { get; set; }
        public bool IsActive { get; set; }
        public string MenuType { get; set; }
        public string Culture { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<MenuItemDto> MenuItems { get; set; } = new List<MenuItemDto>();
    }
} 