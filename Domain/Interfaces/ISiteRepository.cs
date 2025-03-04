using System.Collections.Generic;
using System.Threading.Tasks;
using new_cms.Domain.Entities;
using new_cms.Application.DTOs;

namespace new_cms.Domain.Interfaces
{
    public interface ISiteRepository : IRepository<TAppSite> //site islemleri icin (listeleme, göruntuleme)
    {
        // Tüm sitelerin listesini DTO formatında döndürür. Site yönetim paneli ana sayfası için gerekli.
        Task<IEnumerable<SiteListDto>> GetAllSiteListAsync();

        // Belirtilen site ID'sine ait detaylı bilgileri döndürür. Site düzenleme sayfası için gerekli.
        Task<SiteDetailDto> GetSiteDetailAsync(int id);

        // Şablon olarak işaretlenmiş siteleri listeler. Yeni site oluştururken şablon seçimi için kullanılır.
        Task<IEnumerable<TAppSite>> GetTemplatesAsync();

        // Alan adının benzersiz olup olmadığını kontrol eder. Site oluşturma/düzenleme sırasında validasyon için.
        Task<bool> IsDomainUniqueAsync(string domain, int? excludeSiteId = null);

        // Alan adına göre site bilgisini getirir. Site yönlendirmesi için gerekli.
        Task<TAppSite> GetByDomainAsync(string domain);

        // Yayında olan siteleri listeler. Ön yüz erişimi için gerekli.
        Task<IEnumerable<TAppSite>> GetActiveSitesAsync();

        // Sayfalanmış ve filtrelenmiş site listesini döndürür. Site yönetim paneli listeleme sayfası için.
        Task<(IEnumerable<SiteListDto> Items, int TotalCount)> GetPagedSiteListAsync(
            int pageNumber,
            int pageSize,
            string searchTerm = null,
            string sortBy = null,
            bool ascending = true);
    }
} 