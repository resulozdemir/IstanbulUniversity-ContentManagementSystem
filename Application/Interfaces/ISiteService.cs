using System.Collections.Generic;
using System.Threading.Tasks;
using new_cms.Application.DTOs.SiteDTOs;

namespace new_cms.Application.Interfaces
{
    public interface ISiteService
    {
        // Tüm siteleri listeler
        Task<IEnumerable<SiteListDto>> GetAllSitesAsync();
        
        // Belirli bir site detayını getirir
        Task<SiteDetailDto?> GetSiteByIdAsync(int id);
        
        // Yeni site oluşturur
        Task<SiteDto> CreateSiteAsync(SiteDto siteDto);
        
        // Mevcut siteyi günceller
        Task<SiteDto> UpdateSiteAsync(SiteDto siteDto);
        
        // Siteyi soft delete yapar
        Task DeleteSiteAsync(int id);
        
        // Alan adına göre site getirir
        Task<SiteDetailDto?> GetSiteByDomainAsync(string domain);
        
        // Aktif (yayında) olan siteleri listeler
        Task<IEnumerable<SiteListDto>> GetPublishedSitesAsync();
        
        // Sayfalı ve filtrelenmiş site listesi döndürür
        Task<(IEnumerable<SiteListDto> Items, int TotalCount)> GetPagedSitesAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true);

        // Belirtilen bir şablonu kullanarak yeni bir site oluşturur ve şablon içeriğini kopyalar.
        // siteDto: Kaynak şablon ID'sini (TemplateId alanı üzerinden) ve 
        //          oluşturulacak yeni siteye ait temel bilgileri içeren DTO.
        //          Bu DTO'daki IsTemplate alanı 0 olmalıdır.
        // Döner: Oluşturulan yeni sitenin detaylarını içeren SiteDetailDto.
        Task<SiteDetailDto> CreateSiteFromTemplateAsync(SiteDto siteDto);

        /// Belirtilen ID'ye sahip siteyi yayına alır.
        Task PublishSiteAsync(int siteId);

        /// Belirtilen ID'ye sahip siteyi yayından kaldırır.
        Task UnpublishSiteAsync(int siteId);
    }
} 