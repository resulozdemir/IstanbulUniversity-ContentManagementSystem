using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.EventDTOs
{
    public class EventTagDto
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public int Count { get; set; }
    }
} 