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
        public async Task<IEnumerable<SitePageDto>> GetPagesBySiteIdAsync(int siteId)
        {
            return await _context.TAppSitepages
                .Where(p => p.Siteid == siteId && p.Isdeleted == 0)
                .Select(p => new SitePageDto
                {
                    Id = p.Id,
                    SiteId = p.Siteid,
                    Name = p.Name,
                    TemplateId = p.Templateid,
                    IsDefault = p.Isdefault == 1,
                    Html = p.Html,
                    HtmlDev = p.Htmldev,
                    Style = p.Style,
                    StyleDev = p.Styledev,
                    Javascript = p.Javascript,
                    JavascriptDev = p.Javascriptdev,
                    Routing = p.Routing,            //"/blog/{kategori}/{id}"
                    VirtualPage = p.Virtualpage == 1,   //tek sablon cok ıcerık
                    ReadOnly = p.Readonly == 1,
                    IsDeleted = p.Isdeleted == 1
                })
                .ToListAsync();
        }

        // Belirli bir sayfanın detaylarını getiren metot
        // Sayfa düzenleme ve önizleme ekranlarında kullanılır
        public async Task<SitePageDto> GetPageByIdAsync(int id)
        {
            var page = await _context.TAppSitepages
                .FirstOrDefaultAsync(p => p.Id == id && p.Isdeleted == 0);

            if (page == null)
                return null;

            return new SitePageDto
            {
                Id = page.Id,
                SiteId = page.Siteid,
                Name = page.Name,
                TemplateId = page.Templateid,
                IsDefault = page.Isdefault == 1,
                Html = page.Html,
                HtmlDev = page.Htmldev,
                Style = page.Style,
                StyleDev = page.Styledev,
                Javascript = page.Javascript,
                JavascriptDev = page.Javascriptdev,
                Routing = page.Routing,
                VirtualPage = page.Virtualpage == 1,
                ReadOnly = page.Readonly == 1,
                IsDeleted = page.Isdeleted == 1
            };
        }

        // Belirli bir sitenin varsayılan sayfasını getiren metot
        // Site ana sayfa yönlendirmesi ve varsayılan içerik gösterimi için kullanılır
        public async Task<SitePageDto> GetDefaultPageBySiteIdAsync(int siteId)
        {
            var page = await _context.TAppSitepages
                .FirstOrDefaultAsync(p => p.Siteid == siteId && p.Isdefault == 1 && p.Isdeleted == 0);

            if (page == null)
                return null;

            return new SitePageDto
            {
                Id = page.Id,
                SiteId = page.Siteid,
                Name = page.Name,
                TemplateId = page.Templateid,
                IsDefault = true,
                Html = page.Html,
                HtmlDev = page.Htmldev,
                Style = page.Style,
                StyleDev = page.Styledev,
                Javascript = page.Javascript,
                JavascriptDev = page.Javascriptdev,
                Routing = page.Routing,
                VirtualPage = page.Virtualpage == 1,
                ReadOnly = page.Readonly == 1,
                IsDeleted = page.Isdeleted == 1
            };
        }

        // Sayfanın menülerde veya diğer içeriklerde kullanılıp kullanılmadığını kontrol eden metot
        // Sayfa silme işlemi öncesi bağımlılık kontrolü için kullanılır
        public async Task<bool> IsPageInUseAsync(int id)
        {
            // Sayfanın menülerde kullanımını kontrol et
            return await _context.TAppMenus
                .AnyAsync(m => m.Link.Contains($"/page/{id}") && m.Isdeleted == 0);
        }

        // Belirli bir sitenin sayfa hiyerarşisini ağaç yapısında getiren metot
        // Site haritası, menü yapısı ve içerik organizasyonu için kullanılır
        public async Task<IEnumerable<PageTreeDto>> GetPageTreeBySiteIdAsync(int siteId)
        {
            var pages = await _context.TAppSitepages
                .Where(p => p.Siteid == siteId && p.Isdeleted == 0)
                .OrderBy(p => p.Name)
                .Select(p => new PageTreeDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Routing = p.Routing,
                    IsDefault = p.Isdefault == 1,
                    IsVirtual = p.Virtualpage == 1,
                    TemplateId = p.Templateid,
                    OrderBy = null, // Sıralama için özel bir alan yoksa null bırakılır
                    IsVisible = true, // Görünürlük için özel bir alan yoksa varsayılan true
                    Children = new List<PageTreeDto>() // Alt sayfalar için boş liste
                })
                .ToListAsync();

            return BuildPageHierarchy(pages);
        }

        // Sayfa hiyerarşisini oluşturan yardımcı metot
        // GetPageTreeBySiteIdAsync metodu tarafından kullanılır
        private IEnumerable<PageTreeDto> BuildPageHierarchy(IEnumerable<PageTreeDto> pages)
        {
            // Bu örnekte düz bir liste döndürülüyor
            // Gerçek uygulamada sayfa hiyerarşisini oluşturmak için
            // özel bir mantık eklenebilir
            return pages;
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
        public async Task<SitePageDto> GetPageByRoutingAsync(int siteId, string routing)
        {
            var page = await _context.TAppSitepages
                .FirstOrDefaultAsync(p => p.Siteid == siteId && p.Routing == routing && p.Isdeleted == 0);

            if (page == null)
                return null;

            return new SitePageDto
            {
                Id = page.Id,
                SiteId = page.Siteid,
                Name = page.Name,
                TemplateId = page.Templateid,
                IsDefault = page.Isdefault == 1,
                Html = page.Html,
                HtmlDev = page.Htmldev,
                Style = page.Style,
                StyleDev = page.Styledev,
                Javascript = page.Javascript,
                JavascriptDev = page.Javascriptdev,
                Routing = page.Routing,
                VirtualPage = page.Virtualpage == 1,
                ReadOnly = page.Readonly == 1,
                IsDeleted = page.Isdeleted == 1
            };
        }
    }
} 