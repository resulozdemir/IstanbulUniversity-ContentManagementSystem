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

        // Tüm sitelerin özet bilgilerini liste halinde getiren metot
        // Site yönetim panelinde kullanılır
        public async Task<IEnumerable<SiteListDto>> GetAllSiteListAsync()
        {
            return await _context.TAppSites
                .Where(s => s.Isdeleted == 0)
                .Select(s => new SiteListDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Domain = s.Domain,
                    TemplateId = s.Templateid ?? 0,
                    TemplateName = s.Templateid.HasValue ? 
                        _context.TAppSites.FirstOrDefault(t => t.Id == s.Templateid).Name : "",
                    ThemeId = s.Themeid,
                    ThemeName = _context.TAppThemes.FirstOrDefault(t => t.Id == s.Themeid).Name,
                    Language = s.Language,
                    IsTemplate = s.Istemplate == 1,
                    IsPublish = s.Ispublish == 1,
                    CreatedDate = s.Createddate,
                    ModifiedDate = s.Modifieddate,
                    PageCount = s.TAppSitepages.Count(p => p.Isdeleted == 0),
                    NewsCount = s.TAppNews.Count(n => n.Isdeleted == 0),
                    EventCount = s.TAppEvents.Count(e => e.Isdeleted == 0),
                    ComponentCount = s.TAppSitecomponentdata.Count(c => c.Isdeleted == 0),
                    Domains = s.TAppSitedomains
                        .Where(d => d.Isdeleted == 0)
                        .Select(d => new SiteDomainDto
                        {
                            Id = d.Id,
                            SiteId = d.Siteid,
                            Domain = d.Domain,
                            Language = d.Language,
                            Key = d.Key,
                            AnalyticId = d.Analyticid,
                            GoogleSiteVerification = d.Googlesiteverification
                        }).ToList()
                })
                .ToListAsync();
        }

        // Belirli bir sitenin tüm detaylarını getiren metot
        // Site düzenleme sayfasında kullanılır
        public async Task<SiteDetailDto> GetSiteDetailAsync(int id)
        {
            var site = await _context.TAppSites
                .Include(s => s.TAppSitedomains)
                .Include(s => s.TAppSitepages)
                .Include(s => s.TAppSitecomponentdata)
                .FirstOrDefaultAsync(s => s.Id == id && s.Isdeleted == 0);

            if (site == null)
                return null;

            return new SiteDetailDto
            {
                Id = site.Id,
                Name = site.Name,
                Domain = site.Domain,
                TemplateId = site.Templateid ?? 0,
                TemplateName = site.Templateid.HasValue ? 
                    _context.TAppSites.FirstOrDefault(t => t.Id == site.Templateid).Name : "",
                ThemeId = site.Themeid,
                ThemeName = _context.TAppThemes.FirstOrDefault(t => t.Id == site.Themeid).Name,
                Language = site.Language,
                AnalyticId = site.Analyticid,
                GoogleSiteVerification = site.Googlesiteverification,
                IsTemplate = site.Istemplate == 1,
                IsPublish = site.Ispublish == 1,
                CreatedDate = site.Createddate,
                CreatedUserName = _context.TAuthUsers.FirstOrDefault(u => u.Id == site.Createduser)?.Name ?? "",
                ModifiedDate = site.Modifieddate,
                ModifiedUserName = _context.TAuthUsers.FirstOrDefault(u => u.Id == site.Modifieduser)?.Name ?? "",
                Domains = site.TAppSitedomains
                    .Where(d => d.Isdeleted == 0)
                    .Select(d => new SiteDomainDto
                    {
                        Id = d.Id,
                        SiteId = d.Siteid,
                        Domain = d.Domain,
                        Language = d.Language,
                        Key = d.Key,
                        AnalyticId = d.Analyticid,
                        GoogleSiteVerification = d.Googlesiteverification
                    }).ToList(),
                Pages = site.TAppSitepages
                    .Where(p => p.Isdeleted == 0)
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
                        Routing = p.Routing,
                        VirtualPage = p.Virtualpage == 1,
                        ReadOnly = p.Readonly == 1
                    }).ToList(),
                Components = site.TAppSitecomponentdata
                    .Where(c => c.Isdeleted == 0)
                    .Select(c => new SiteComponentDataDto
                    {
                        Id = c.Id,
                        SiteId = c.Siteid,
                        ThemeComponentId = c.Themecomponentid,
                        Data = c.Data,
                        Column1 = c.Column1,
                        Column2 = c.Column2,
                        Column3 = c.Column3,
                        Column4 = c.Column4
                    }).ToList(),
                AvailableComponents = _context.TAppThemecomponents
                    .Where(tc => tc.Themeid == site.Themeid && tc.Isdeleted == 0)
                    .Select(tc => new ThemeComponentDto
                    {
                        Id = tc.Id,
                        ThemeId = tc.Themeid ?? 0,
                        ComponentId = tc.Componentid,
                        Name = tc.Name,
                        Description = tc.Description,
                        Template = tc.Template,
                        Style = tc.Style,
                        Javascript = tc.Javascript,
                        FormJson = tc.Formjson,
                        FormHtml = tc.Formhtml,
                        FormJs = tc.Formjs
                    }).ToList()
            };
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
        public async Task<TAppSite> GetByDomainAsync(string domain)
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
        public async Task<(IEnumerable<SiteListDto> Items, int TotalCount)> GetPagedSiteListAsync(
            int pageNumber,
            int pageSize,
            string searchTerm = null,
            string sortBy = null,
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

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new SiteListDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Domain = s.Domain,
                    TemplateId = s.Templateid ?? 0,
                    TemplateName = s.Templateid.HasValue ?
                        _context.TAppSites.FirstOrDefault(t => t.Id == s.Templateid).Name : "",
                    ThemeId = s.Themeid,
                    ThemeName = _context.TAppThemes.FirstOrDefault(t => t.Id == s.Themeid).Name,
                    Language = s.Language,
                    IsTemplate = s.Istemplate == 1,
                    IsPublish = s.Ispublish == 1,
                    CreatedDate = s.Createddate,
                    ModifiedDate = s.Modifieddate,
                    PageCount = s.TAppSitepages.Count(p => p.Isdeleted == 0),
                    NewsCount = s.TAppNews.Count(n => n.Isdeleted == 0),
                    EventCount = s.TAppEvents.Count(e => e.Isdeleted == 0),
                    ComponentCount = s.TAppSitecomponentdata.Count(c => c.Isdeleted == 0),
                    Domains = s.TAppSitedomains
                        .Where(d => d.Isdeleted == 0)
                        .Select(d => new SiteDomainDto
                        {
                            Id = d.Id,
                            SiteId = d.Siteid,
                            Domain = d.Domain,
                            Language = d.Language,
                            Key = d.Key,
                            AnalyticId = d.Analyticid,
                            GoogleSiteVerification = d.Googlesiteverification
                        }).ToList()
                })
                .ToListAsync();

            return (items, totalCount);
        }
    }
} 