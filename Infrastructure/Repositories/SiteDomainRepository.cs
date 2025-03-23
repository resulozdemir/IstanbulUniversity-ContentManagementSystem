using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;
using new_cms.Application.DTOs;

namespace new_cms.Infrastructure.Persistence.Repositories
{
    /// Site alan adı yönetimi için gerekli veritabanı işlemlerini gerçekleştiren repository sınıfı.
    /// Alan adı ekleme, düzenleme, listeleme ve dil bazlı alan adı yönetimi işlemlerini gerçekleştirir.
    public class SiteDomainRepository : BaseRepository<TAppSitedomain>, ISiteDomainRepository
    {
        public SiteDomainRepository(UCmsContext context) : base(context)
        {
        }

        // Belirli bir siteye ait tüm alan adlarını getiren metot
        // Site alan adı yönetim sayfasında kullanılır
        public async Task<IEnumerable<TAppSitedomain>> GetDomainsBySiteIdAsync(int siteId)
        {
            return await _context.TAppSitedomains
                .Where(d => d.Siteid == siteId && d.Isdeleted == 0)
                .ToListAsync();
        }

        // Belirli bir alan adının detaylarını getiren metot
        // Alan adı düzenleme sayfasında kullanılır
        public async Task<TAppSitedomain> GetDomainByIdAsync(int id)
        {
            return await _context.TAppSitedomains
                .FirstOrDefaultAsync(d => d.Id == id && d.Isdeleted == 0);
        }

        // Alan adının benzersiz olup olmadığını kontrol eden metot
        // Alan adı ekleme ve düzenleme işlemlerinde validasyon için kullanılır
        public async Task<bool> IsDomainUniqueAsync(string domain, int? excludeDomainId = null)
        {
            var query = _context.TAppSitedomains
                .Where(d => d.Domain == domain && d.Isdeleted == 0);

            if (excludeDomainId.HasValue)
            {
                query = query.Where(d => d.Id != excludeDomainId.Value);
            }

            return !await query.AnyAsync();
        }

        // Alan adına göre kayıt getiren metot
        // Site yönlendirmesi ve alan adı doğrulama işlemlerinde kullanılır
        public async Task<TAppSitedomain> GetByDomainAsync(string domain)
        {
            return await _context.TAppSitedomains
                .FirstOrDefaultAsync(d => d.Domain == domain && d.Isdeleted == 0);
        }

        // Belirli bir dile ait tüm alan adlarını getiren metot
        // Dil bazlı alan adı yönetimi ve çoklu dil desteği için kullanılır
        public async Task<IEnumerable<TAppSitedomain>> GetDomainsByLanguageAsync(string language)
        {
            return await _context.TAppSitedomains
                .Where(d => d.Language == language && d.Isdeleted == 0)
                .ToListAsync();
        }
    }
} 