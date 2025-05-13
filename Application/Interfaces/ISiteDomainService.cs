using System.Collections.Generic;
using System.Threading.Tasks;
using new_cms.Application.DTOs.SiteDTOs;

namespace new_cms.Application.Interfaces
{
    public interface ISiteDomainService
    {
        // Belirli bir site için tüm domain'leri listeler
        Task<IEnumerable<SiteDomainDto>> GetDomainsBySiteIdAsync(int siteId);
        
        // Yeni domain ekler
        Task<SiteDomainDto> CreateDomainAsync(SiteDomainDto domainDto);
        
        // Mevcut domain'i günceller
        Task<SiteDomainDto> UpdateDomainAsync(SiteDomainDto domainDto);
        
        // Domain'i soft delete yapar
        Task DeleteDomainAsync(int id);
        
        // Domain isminin benzersiz olup olmadığını kontrol eder
        Task<bool> IsDomainUniqueAsync(string domain, int? excludeDomainId = null);
        
        // Domain adına göre kayıt getirir
        Task<SiteDomainDto?> GetByDomainAsync(string domain);
        
    }
} 