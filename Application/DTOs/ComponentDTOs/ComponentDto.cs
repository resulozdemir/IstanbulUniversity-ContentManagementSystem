using System;

namespace new_cms.Application.DTOs.ComponentDTOs
{
    /// Bileşen bilgilerini taşıyan DTO
    public class ComponentDto
    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Template { get; set; } = string.Empty;
        public string Style { get; set; } = string.Empty;
        public string Script { get; set; } = string.Empty;
        public string FormJson { get; set; } = string.Empty;
        public int ComponentType { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
