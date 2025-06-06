using System;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.MenuDTOs
{
    /// Menü oluşturma ve güncelleme işlemleri için kullanılan DTO
    public class MenuDto
    {
        public int? Id { get; set; }  // Create için null olabilir
        
        [Required]
        [StringLength(250)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public int SiteId { get; set; }
        
        public int? ParentId { get; set; }
        
        [StringLength(200)]
        public string? Link { get; set; }
        
        public int? MenuOrder { get; set; }
        
        [StringLength(40)]
        public string? Icon { get; set; }
        
        public int? Status { get; set; }
        
        public int? Type { get; set; }
        
        public int GroupId { get; set; }
        
        [StringLength(40)]
        public string? Target { get; set; }
        
        public int IsDeleted { get; set; } = 0;
    }
} 