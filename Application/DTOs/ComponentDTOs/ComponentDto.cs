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
        public string Formjson { get; set; } = string.Empty;
        public string TagName { get; set; } = string.Empty;
        public string Column1 { get; set; } = string.Empty;
        public string Column2 { get; set; } = string.Empty;
        public string Column3 { get; set; } = string.Empty;
        public string Column4 { get; set; } = string.Empty;
    }
}
