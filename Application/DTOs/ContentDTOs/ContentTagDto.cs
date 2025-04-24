using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.ContentDTOs
{
    public class ContentTagDto
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public int Count { get; set; }
    }
} 