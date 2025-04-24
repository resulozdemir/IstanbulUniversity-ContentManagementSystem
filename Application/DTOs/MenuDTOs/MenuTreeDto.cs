using System.Collections.Generic;

namespace new_cms.Application.DTOs.MenuDTOs
{
    public class MenuTreeDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
        public int? SortOrder { get; set; }
        public string Icon { get; set; }
        public string Target { get; set; }
        public List<MenuTreeDto> Children { get; set; } = new List<MenuTreeDto>();
    }
} 