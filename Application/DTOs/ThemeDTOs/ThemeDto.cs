using System;

namespace new_cms.Application.DTOs.ThemeDTOs
{
    /// Tema bilgilerini taşıyan DTO
    public class ThemeDto
    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
    }
} 