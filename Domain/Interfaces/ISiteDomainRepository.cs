using System.Collections.Generic;
using System.Threading.Tasks;
using new_cms.Domain.Entities;
using new_cms.Application.DTOs;

namespace new_cms.Domain.Interfaces
{
    public interface ISiteDomainRepository : IRepository<TAppSitedomain> //alan adı yönetimi için repository
    {
        // Belirtilen site ID'sine ait tüm alan adlarını listeler. Site alan adı yönetimi için gerekli.
        Task<IEnumerable<TAppSitedomain>> GetDomainsBySiteIdAsync(int siteId);

        // Belirtilen alan adı ID'sine ait detayları döndürür. Alan adı düzenleme sayfası için gerekli.
        Task<TAppSitedomain> GetDomainByIdAsync(int id);

        // Alan adının benzersiz olup olmadığını kontrol eder. Alan adı ekleme/düzenleme sırasında validasyon için.
        Task<bool> IsDomainUniqueAsync(string domain, int? excludeDomainId = null);

        // Alan adı bilgisine göre kayıt getirir. Site yönlendirmesi için gerekli.
        Task<TAppSitedomain> GetByDomainAsync(string domain);

        // Belirtilen dildeki tüm alan adlarını listeler. Dil bazlı alan adı yönetimi için gerekli.
        Task<IEnumerable<TAppSitedomain>> GetDomainsByLanguageAsync(string language);
    }
} 