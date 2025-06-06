using System;
using System.ComponentModel.DataAnnotations;

namespace new_cms.Application.DTOs.SitemapDTOs
{
    /// Site haritası ve URL yönetimi için kullanılan DTO
    public class SitemapDto
    {
        public int? Id { get; set; }  // Create için null olabilir

        [Required]
        [StringLength(2000)]
        public string Url { get; set; } = string.Empty;  // URL adresi

        [StringLength(200)]
        public string? Domain { get; set; }  // Domain adı

        [StringLength(10)]
        public string? Lang { get; set; }  // Dil kodu (tr, en, vs.)

        public int? SiteId { get; set; }  // Site ID'si (nullable)

        public int? ItemId { get; set; }  // İçerik item ID'si (haber, sayfa vs.)

        public int? Column1 { get; set; }  // İçerik tipi (1:haber, 2:sayfa, 3:ürün vs.)

        [StringLength(100)]
        public string? Column2 { get; set; }  // Kategori veya ek bilgi

        [StringLength(2000)]
        public string? RedirectTo { get; set; }  // Yönlendirme URL'i

        public int Active { get; set; } = 1;  // Aktif/pasif durumu (0: pasif, 1: aktif)

        public int? CreatedUser { get; set; }  // Oluşturan kullanıcı

        public int? ModifiedUser { get; set; }  // Güncelleyen kullanıcı

        public int IsDeleted { get; set; } = 0;  // Silme durumu (0: aktif, 1: silinmiş)
    }
} 