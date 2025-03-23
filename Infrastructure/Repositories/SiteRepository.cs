using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;
using new_cms.Application.DTOs;

namespace new_cms.Infrastructure.Persistence.Repositories
{
    /// Site yönetimi için gerekli veritabanı işlemlerini gerçekleştiren repository sınıfı.
    /// Site oluşturma, düzenleme, listeleme ve silme işlemlerini yönetir.
    public class SiteRepository : BaseRepository<TAppSite>, ISiteRepository
    {
        public SiteRepository(UCmsContext context) : base(context)
        {
        }

        // Tüm sitelerin listesini getiren metot
        // Site yönetim panelinde kullanılır
        public async Task<IEnumerable<TAppSite>> GetAllSiteListAsync()
        {
            return await _context.TAppSites
                .Where(s => s.Isdeleted == 0)
                .Include(s => s.TAppSitedomains.Where(d => d.Isdeleted == 0))
                .Include(s => s.TAppSitepages.Where(p => p.Isdeleted == 0))
                .Include(s => s.TAppNews.Where(n => n.Isdeleted == 0))
                .Include(s => s.TAppEvents.Where(e => e.Isdeleted == 0))
                .Include(s => s.TAppSitecomponentdata.Where(c => c.Isdeleted == 0))
                .ToListAsync();
        }

        // Belirli bir sitenin tüm detaylarını getiren metot
        // Site düzenleme sayfasında kullanılır
        public async Task<TAppSite?> GetSiteDetailAsync(int id)
        {
            return await _context.TAppSites
                .Include(s => s.TAppSitedomains.Where(d => d.Isdeleted == 0))
                .Include(s => s.TAppSitepages.Where(p => p.Isdeleted == 0))
                .Include(s => s.TAppSitecomponentdata.Where(c => c.Isdeleted == 0))
                .FirstOrDefaultAsync(s => s.Id == id && s.Isdeleted == 0);
        }

        // Şablon olarak işaretlenmiş siteleri getiren metot
        // Yeni site oluştururken şablon seçimi için kullanılır
        public async Task<IEnumerable<TAppSite>> GetTemplatesAsync()
        {
            return await _dbSet
                .Where(s => s.Istemplate == 1 && s.Isdeleted == 0)
                .ToListAsync();
        }

        // Alan adının benzersiz olup olmadığını kontrol eden metot
        // Site oluşturma ve düzenleme sırasında kullanılır
        public async Task<bool> IsDomainUniqueAsync(string domain, int? excludeSiteId = null)
        {
            var query = _context.TAppSitedomains
                .Where(d => d.Isdeleted == 0 && d.Domain == domain);

            if (excludeSiteId.HasValue)
            {
                query = query.Where(d => d.Siteid != excludeSiteId.Value);
            }

            return !await query.AnyAsync();
        }

        // Alan adına göre site bilgisini getiren metot
        // Site yönlendirmesi için kullanılır
        public async Task<TAppSite?> GetByDomainAsync(string domain)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => 
                    (s.Domain == domain || s.TAppSitedomains.Any(d => d.Domain == domain && d.Isdeleted == 0)) 
                    && s.Isdeleted == 0);
        }

        // Yayında olan siteleri getiren metot
        // Ön yüz erişimi için kullanılır
        public async Task<IEnumerable<TAppSite>> GetActiveSitesAsync()
        {
            return await _dbSet
                .Where(s => s.Isdeleted == 0 && s.Ispublish == 1)
                .ToListAsync();
        }

        // Sayfalanmış ve filtrelenmiş site listesini getiren metot
        // Site yönetim paneli listeleme sayfasında kullanılır
        public async Task<(IEnumerable<TAppSite> Items, int TotalCount)> GetPagedSiteListAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true)
        {
            var query = _context.TAppSites
                .Where(s => s.Isdeleted == 0);

            // Arama filtresi uygulama
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(s =>
                    s.Name.Contains(searchTerm) ||
                    s.Domain.Contains(searchTerm) ||
                    s.TAppSitedomains.Any(d => d.Domain.Contains(searchTerm)));
            }

            // Sıralama uygulama
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "name" => ascending ? query.OrderBy(s => s.Name) : query.OrderByDescending(s => s.Name),
                    "domain" => ascending ? query.OrderBy(s => s.Domain) : query.OrderByDescending(s => s.Domain),
                    "createddate" => ascending ? query.OrderBy(s => s.Createddate) : query.OrderByDescending(s => s.Createddate),
                    _ => query.OrderBy(s => s.Id)
                };
            }
            else
            {
                query = query.OrderByDescending(s => s.Createddate);
            }

            var totalCount = await query.CountAsync();

            // Site ile ilişkili verileri include et
            query = query
                .Include(s => s.TAppSitedomains.Where(d => d.Isdeleted == 0))
                .Include(s => s.TAppSitepages.Where(p => p.Isdeleted == 0))
                .Include(s => s.TAppSitecomponentdata.Where(c => c.Isdeleted == 0));

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
} 