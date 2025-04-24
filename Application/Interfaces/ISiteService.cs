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
        
        // Site şablonlarını listeler (template olarak işaretlenmiş siteler)
        Task<IEnumerable<SiteListDto>> GetSiteTemplatesAsync();
        
        // Alan adına göre site getirir
        Task<SiteDetailDto?> GetSiteByDomainAsync(string domain);
        
        // Aktif (yayında) olan siteleri listeler
        Task<IEnumerable<SiteListDto>> GetActiveSitesAsync();
        
        // Sayfalı ve filtrelenmiş site listesi döndürür
        Task<(IEnumerable<SiteListDto> Items, int TotalCount)> GetPagedSitesAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true);
            
        // Alan adının benzersiz olup olmadığını kontrol eder
        Task<bool> IsDomainUniqueAsync(string domain, int? excludeSiteId = null);
    }
} 