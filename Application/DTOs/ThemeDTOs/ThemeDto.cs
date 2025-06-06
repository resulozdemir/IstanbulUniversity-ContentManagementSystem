using System;

namespace new_cms.Application.DTOs.ThemeDTOs
{
    /// Tema bilgilerini taşıyan DTO
    public class ThemeDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; } // StringLength(100), Unicode(false) TAppTheme'den
        public string? Header { get; set; }
        public string? Footer { get; set; }
        public int IsDeleted { get; set; } = 0;
    }
} 