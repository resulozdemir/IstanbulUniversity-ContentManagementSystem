using new_cms.Application.DTOs.SitemapDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.Application.Interfaces
{
    /// Site haritası (TAppSitemap) yönetimi ile ilgili operasyonları tanımlayan arayüz
    public interface ISitemapService
    {
        /// Sayfalı ve filtrelenmiş site haritası listesi getirir
        Task<(IEnumerable<SitemapListDto> Items, int TotalCount)> GetPagedSitemapsAsync(
            int pageNumber, 
            int pageSize, 
            int? siteId = null,
            string? domain = null,
            string? lang = null,
            int? column1 = null,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true);

        /// Belirtilen ID'ye sahip site haritası kaydını getirir
        Task<SitemapDto?> GetSitemapByIdAsync(int id);

        /// Sistemdeki tüm aktif site haritası kayıtlarını getirir
        Task<IEnumerable<SitemapListDto>> GetActiveSitemapsAsync();

        /// Belirtilen site'ye ait site haritası kayıtlarını getirir
        Task<IEnumerable<SitemapListDto>> GetSitemapsBySiteIdAsync(int siteId);

        /// Belirtilen domain'e ait site haritası kayıtlarını getirir
        Task<IEnumerable<SitemapListDto>> GetSitemapsByDomainAsync(string domain);

        /// Belirtilen dile ait site haritası kayıtlarını getirir
        Task<IEnumerable<SitemapListDto>> GetSitemapsByLangAsync(string lang);

        /// Belirtilen URL için site haritası kaydını getirir
        Task<SitemapDto?> GetSitemapByUrlAsync(string url);

        /// Yeni bir site haritası kaydı oluşturur
        Task<SitemapDto> CreateSitemapAsync(SitemapDto sitemapDto);

        /// Mevcut bir site haritası kaydını günceller
        Task<SitemapDto> UpdateSitemapAsync(SitemapDto sitemapDto);

        /// Site haritası kaydını siler (soft delete)
        Task DeleteSitemapAsync(int id);

        /// Belirtilen içerik tipi (Column1) için site haritası kayıtlarını getirir
        Task<IEnumerable<SitemapListDto>> GetSitemapsByContentTypeAsync(int column1);
    }
} 