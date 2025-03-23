using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;
using new_cms.Application.DTOs;

namespace new_cms.Infrastructure.Persistence.Repositories
{
    /// Site sayfa yönetimi için gerekli veritabanı işlemlerini gerçekleştiren repository sınıfı.
    /// Sayfa oluşturma, düzenleme, listeleme, yönlendirme ve sayfa hiyerarşisi yönetimi işlemlerini gerçekleştirir.
    public class SitePageRepository : BaseRepository<TAppSitepage>, ISitePageRepository
    {
        public SitePageRepository(UCmsContext context) : base(context)
        {
        }

        // Belirli bir siteye ait tüm sayfaları getiren metot
        // Site içerik yönetimi ve sayfa listeleme ekranında kullanılır
        public async Task<IEnumerable<TAppSitepage>> GetPagesBySiteIdAsync(int siteId)
        {
            return await _context.TAppSitepages
                .Where(p => p.Siteid == siteId && p.Isdeleted == 0)
                .ToListAsync();
        }

        // Belirli bir sayfanın detaylarını getiren metot
        // Sayfa düzenleme ve önizleme ekranlarında kullanılır
        public async Task<TAppSitepage?> GetPageByIdAsync(int id)
        {
            return await _context.TAppSitepages
                .FirstOrDefaultAsync(p => p.Id == id && p.Isdeleted == 0);
        }

        // Belirli bir sitenin varsayılan sayfasını getiren metot
        // Site ana sayfa yönlendirmesi ve varsayılan içerik gösterimi için kullanılır
        public async Task<TAppSitepage?> GetDefaultPageBySiteIdAsync(int siteId)
        {
            return await _context.TAppSitepages
                .FirstOrDefaultAsync(p => p.Siteid == siteId && p.Isdefault == 1 && p.Isdeleted == 0);
        }

        // Sayfanın menülerde veya diğer içeriklerde kullanılıp kullanılmadığını kontrol eden metot
        // Sayfa silme işlemi öncesi bağımlılık kontrolü için kullanılır
        public async Task<bool> IsPageInUseAsync(int id)
        {
            // Sayfanın menülerde kullanımını kontrol et
            return await _context.TAppMenus
                .AnyAsync(m => m.Link.Contains($"/page/{id}") && m.Isdeleted == 0);
        }

        // Belirli bir sitenin sayfalarını getiren metot
        // Site haritası, menü yapısı ve içerik organizasyonu için kullanılır
        public async Task<IEnumerable<TAppSitepage>> GetPageTreeBySiteIdAsync(int siteId)
        {
            return await _context.TAppSitepages
                .Where(p => p.Siteid == siteId && p.Isdeleted == 0)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        // Sayfa yönlendirme adresinin benzersiz olup olmadığını kontrol eden metot
        // Sayfa ekleme ve düzenleme işlemlerinde URL çakışmalarını önlemek için kullanılır
        public async Task<bool> IsRoutingUniqueAsync(int siteId, string routing, int? excludePageId = null)
        {
            var query = _context.TAppSitepages
                .Where(p => p.Siteid == siteId && p.Routing == routing && p.Isdeleted == 0);

            if (excludePageId.HasValue)
            {
                query = query.Where(p => p.Id != excludePageId.Value);
            }

            return !await query.AnyAsync();
        }

        // Belirli bir yönlendirme adresine sahip sayfayı getiren metot
        // URL tabanlı sayfa yönlendirmesi ve içerik gösterimi için kullanılır
        public async Task<TAppSitepage?> GetPageByRoutingAsync(int siteId, string routing)
        {
            return await _context.TAppSitepages
                .FirstOrDefaultAsync(p => p.Siteid == siteId && p.Routing == routing && p.Isdeleted == 0);
        }
    }
} 