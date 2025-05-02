using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using new_cms.Application.DTOs.SiteDTOs;
using new_cms.Application.Interfaces;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;

namespace new_cms.Application.Services
{
    public class SiteService : ISiteService
    {
        private readonly IRepository<TAppSite> _siteRepository;
        private readonly IRepository<TAppTheme> _themeRepository;
        private readonly ISiteDomainService _siteDomainService;
        private readonly IMapper _mapper;

        public SiteService(
            IRepository<TAppSite> siteRepository,
            IRepository<TAppTheme> themeRepository,
            ISiteDomainService siteDomainService,
            IMapper mapper)
        {
            _siteRepository = siteRepository;
            _themeRepository = themeRepository;
            _siteDomainService = siteDomainService;
            _mapper = mapper;
        }

        // Tüm siteleri listeler
        public async Task<IEnumerable<SiteListDto>> GetAllSitesAsync()
        {
            try
            {
                var sites = await _siteRepository.Query()
                    .Where(s => s.Isdeleted == 0)
                    .Include(s => s.TAppSitedomains.Where(d => d.Isdeleted == 0)) 
                    .ToListAsync();

                var siteDtos = _mapper.Map<IEnumerable<SiteListDto>>(sites);

                return siteDtos;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Site listesi alınırken hata: {ex.Message}", ex);
            }
        }

        // Site detayı getir 
        public async Task<SiteDetailDto?> GetSiteByIdAsync(int id)
        {
            try
            {
                var site = await _siteRepository.Query()
                    .Include(s => s.TAppSitedomains.Where(d => d.Isdeleted == 0))
                    .Include(s => s.TAppSitepages.Where(p => p.Isdeleted == 0)) 
                    .Include(s => s.TAppNews.Where(n => n.Isdeleted == 0))
                    .Include(s => s.TAppEvents.Where(e => e.Isdeleted == 0))
                    .Include(s => s.TAppSitecomponentdata.Where(c => c.Isdeleted == 0))
                    .FirstOrDefaultAsync(s => s.Id == id && s.Isdeleted == 0);

                if (site == null)
                    return null;

                var siteDto = _mapper.Map<SiteDetailDto>(site);

                // Tema adını doldur
                if (site.Themeid > 0)
                {
                    var theme = await _themeRepository.GetByIdAsync(site.Themeid);
                    if (theme != null && theme.Isdeleted == 0)
                    {
                        siteDto.ThemeName = theme.Name;
                    }
                }

                // Domain bilgilerini doldur
                siteDto.Domains = _mapper.Map<List<SiteDomainDto>>(site.TAppSitedomains);

                return siteDto;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Site detayı alınırken hata (ID: {id}): {ex.Message}", ex);
            }
        }

        // Yeni site oluştur
        public async Task<SiteDto> CreateSiteAsync(SiteDto siteDto)
        {
            if (string.IsNullOrWhiteSpace(siteDto.Domain))
            {
                 throw new ArgumentException("Site için birincil domain adı boş olamaz.", nameof(siteDto.Domain));
            }

            if (!await _siteDomainService.IsDomainUniqueAsync(siteDto.Domain))
            {
                throw new InvalidOperationException($"'{siteDto.Domain}' alan adı zaten kullanılıyor.");
            }

            var site = _mapper.Map<TAppSite>(siteDto);

            // Zorunlu alanlar ve varsayılan değerler
            site.Isdeleted = 0;                     
            site.Ispublish = 0;                    
            site.Createddate = DateTime.UtcNow;     
            // site.Createduser = GetCurrentUserId(); // TODO: Aktif kullanıcı ID'si alınmalı

            var createdSite = await _siteRepository.AddAsync(site);

            var siteDomainDto = new SiteDomainDto
            {
                SiteId = createdSite.Id,
                Domain = createdSite.Domain ?? string.Empty,
                Language = createdSite.Language ?? "tr",
                AnalyticId = createdSite.Analyticid,
                GoogleSiteVerification = createdSite.Googlesiteverification
            };

            await _siteDomainService.CreateDomainAsync(siteDomainDto);

            return _mapper.Map<SiteDto>(createdSite);
        }

        // Site bilgilerini güncelle
        public async Task<SiteDto> UpdateSiteAsync(SiteDto siteDto)
        {
            if (!siteDto.Id.HasValue || siteDto.Id <= 0)
                throw new ArgumentException("Güncelleme için geçerli bir Site ID gereklidir.", nameof(siteDto.Id));

            var existingSite = await _siteRepository.GetByIdAsync(siteDto.Id.Value);

            if (existingSite == null || existingSite.Isdeleted == 1)
                throw new KeyNotFoundException($"Site bulunamadı veya silinmiş: ID {siteDto.Id.Value}");

            // AutoMapper'ın üzerine yazmaması gereken alanları sakla
            var originalIsDeleted = existingSite.Isdeleted;
            var originalCreatedDate = existingSite.Createddate;
            //var originalCreatedUser = existingSite.Createduser; 

            _mapper.Map(siteDto, existingSite);

            // Saklanan orijinal değerleri geri yükle
            existingSite.Isdeleted = originalIsDeleted;
            existingSite.Createddate = originalCreatedDate;
            //existingSite.Createduser = originalCreatedUser; 

            // Güncelleme bilgilerini ayarla
            existingSite.Modifieddate = DateTime.UtcNow;
            // existingSite.Modifieduser = GetCurrentUserId(); // TODO: Aktif kullanıcı ID'si alınmalı

            await _siteRepository.UpdateAsync(existingSite);

            return _mapper.Map<SiteDto>(existingSite);
        }

        // Site silme
        public async Task DeleteSiteAsync(int id)
        {
             try
            {
                await _siteRepository.SoftDeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Site silinirken hata (ID: {id}): {ex.Message}", ex);
            }
        }

        // Site şablonlarını listele
        public async Task<IEnumerable<SiteListDto>> GetSiteTemplatesAsync()
        {
            var templates = await _siteRepository.Query()
                .Where(s => s.Istemplate == 1 && s.Isdeleted == 0)
                .ToListAsync();

            var templateDtos = _mapper.Map<IEnumerable<SiteListDto>>(templates);
            return templateDtos;
        }

        // Alan adına göre site bilgisi getir
        public async Task<SiteDetailDto?> GetSiteByDomainAsync(string domain)
        {
            var domainDto = await _siteDomainService.GetByDomainAsync(domain);
            if (domainDto?.SiteId == null)
                return null;

            return await GetSiteByIdAsync(domainDto.SiteId);
        }

        // Published (yayında) olan siteleri listele
        public async Task<IEnumerable<SiteListDto>> GetPublishedSitesAsync()
        {
            var sites = await _siteRepository.Query()
                .Where(s => s.Ispublish == 1 && s.Isdeleted == 0)
                .ToListAsync();

            var siteDtos = _mapper.Map<IEnumerable<SiteListDto>>(sites);
            return siteDtos;
        }

        // Sayfalı ve filtrelenmiş site listesi döndür
        public async Task<(IEnumerable<SiteListDto> Items, int TotalCount)> GetPagedSitesAsync(
            int pageNumber,
            int pageSize)
        {
            var query = _siteRepository.Query()
                           .Where(s => s.Isdeleted == 0);

            // Sıralama
            query = query.OrderByDescending(s => s.Createddate);

            // Toplam sayıyı al
            var totalCount = await query.CountAsync();

            // Sayfalama uygula
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

             var siteDtos = _mapper.Map<IEnumerable<SiteListDto>>(items);

            return (siteDtos, totalCount);
        }

        /// Belirli bir template kullanan aktif siteleri listeler.
        public async Task<IEnumerable<SiteListDto>> GetSitesByTemplateAsync(int templateId)
        {
            try
            {
                var sites = await _siteRepository.Query()
                    .Where(s => s.Templateid == templateId && s.Isdeleted == 0 && s.Istemplate == 0) // Şablonun kendisini değil, onu kullananları getir
                    .ToListAsync();

                var siteDtos = _mapper.Map<IEnumerable<SiteListDto>>(sites);
                return siteDtos;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Şablona göre siteler listelenirken hata oluştu (Template ID: {templateId}).", ex);
            }
        }

        /// Belirtilen ID'ye sahip siteyi yayına alır (Ispublish = 1).
        public async Task PublishSiteAsync(int siteId)
        {
            try
            {
                var site = await _siteRepository.GetByIdAsync(siteId);
                if (site == null || site.Isdeleted == 1)
                {
                    throw new KeyNotFoundException($"Yayınlanacak site bulunamadı veya silinmiş: ID {siteId}");
                }

                if (site.Ispublish == 1)
                {
                    return; 
                }

                site.Ispublish = 1;
                site.Modifieddate = DateTime.UtcNow;
                // site.Modifieduser = GetCurrentUserId();

                await _siteRepository.UpdateAsync(site);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Site yayınlanırken veritabanı hatası oluştu (ID: {siteId}).", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Site yayınlanırken beklenmedik bir hata oluştu (ID: {siteId}).", ex);
            }
        }

        /// Belirtilen ID'ye sahip siteyi yayından kaldırır (Ispublish = 0).
        public async Task UnpublishSiteAsync(int siteId)
        {
             try
            {
                var site = await _siteRepository.GetByIdAsync(siteId);
                if (site == null || site.Isdeleted == 1)
                {
                    throw new KeyNotFoundException($"Yayından kaldırılacak site bulunamadı veya silinmiş: ID {siteId}");
                }

                if (site.Ispublish == 0)
                {
                    return;
                }

                site.Ispublish = 0;
                site.Modifieddate = DateTime.UtcNow;
                // site.Modifieduser = GetCurrentUserId(); // TODO

                await _siteRepository.UpdateAsync(site);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Site yayından kaldırılırken veritabanı hatası oluştu (ID: {siteId}).", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Site yayından kaldırılırken beklenmedik bir hata oluştu (ID: {siteId}).", ex);
            }
        }
    }
} 