using System;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.ComponentDTOs
{
    /// Site bileşen verisini taşıyan DTO
    public class SiteComponentDataDto //site'nin bileşen verileri (header slider, news, event, content, menu)
    {
        public int? Id { get; set; }  // Create için null olabilir

        [Required]
        public int SiteId { get; set; }  // Site ID'si

        [Required]
        public int ThemeComponentId { get; set; }  // Tema bileşeni ID'si
        
        public int? ComponentId { get; set; }  // Bileşen ID'si (ilişkili tablodaki)
        
        public string? ComponentName { get; set; }  // Bileşenin adı (ilişkili tablodaki)

        public string? Data { get; set; }  // Bileşen verisi (HTML, JSON vb.)

        public string? Column1 { get; set; }  // Ek veri alanı 1

        public string? Column2 { get; set; }  // Ek veri alanı 2

        public string? Column3 { get; set; }  // Ek veri alanı 3

        public string? Column4 { get; set; }  // Ek veri alanı 4

        public DateTime? CreatedDate { get; set; }
        
        public int? CreatedUser { get; set; }
        
        public DateTime? ModifiedDate { get; set; }
        
        public int? ModifiedUser { get; set; }

        public int IsDeleted { get; set; } = 0;  // Varsayılan olarak 0

        public string DataJson { get; set; } = string.Empty;
        public string Settings { get; set; } = string.Empty;
        public int PageId { get; set; }
        public int OrderBy { get; set; }
        public bool IsActive { get; set; } = true;
    }
} 