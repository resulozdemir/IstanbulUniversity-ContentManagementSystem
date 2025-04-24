using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using new_cms.Application.DTOs.SiteDTOs;
using new_cms.Application.Interfaces;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;

namespace new_cms.Application.Services
{
    public class SiteService : ISiteService
    {
        private readonly ISiteRepository _siteRepository;
        private readonly IThemeRepository _themeRepository;
        private readonly ISiteDomainService _siteDomainService;
        private readonly IMapper _mapper;

        public SiteService(
            ISiteRepository siteRepository, 
            IThemeRepository themeRepository,
            ISiteDomainService siteDomainService,
            IMapper mapper)
        {
            _siteRepository = siteRepository ?? throw new ArgumentNullException(nameof(siteRepository));
            _themeRepository = themeRepository ?? throw new ArgumentNullException(nameof(themeRepository));
            _siteDomainService = siteDomainService ?? throw new ArgumentNullException(nameof(siteDomainService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // Tüm siteleri listeler
        public async Task<IEnumerable<SiteListDto>> GetAllSitesAsync()
        {
            try
            {
                // Site repository'den tüm site bilgilerini çek
                var sites = await _siteRepository.GetAllSiteListAsync();
                
                // DTO'ya dönüştür
                var siteDtos = _mapper.Map<IEnumerable<SiteListDto>>(sites);
                
                // Tema adlarını doldur
                foreach (var site in siteDtos)
                {
                    if (site.ThemeId > 0)
                    {
                        var theme = await _themeRepository.GetByIdAsync(site.ThemeId);
                        if (theme != null)
                        {
                            site.ThemeName = theme.Name;
                        }
                    }
                }
                
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
                // Site bilgisini getir
                var site = await _siteRepository.GetSiteDetailAsync(id);
                if (site == null)
                    return null;
                    
                // DTO'ya dönüştür
                var siteDto = _mapper.Map<SiteDetailDto>(site);
                
                // Tema adını doldur
                if (site.Themeid > 0)
                {
                    var theme = await _themeRepository.GetByIdAsync(site.Themeid);
                    if (theme != null)
                    {
                        siteDto.ThemeName = theme.Name;
                    }
                }
                
                // Domain bilgilerini doldur - SiteDomainService'i kullanarak
                var domains = await _siteDomainService.GetDomainsBySiteIdAsync(id);
                siteDto.Domains = domains.ToList();
                
                return siteDto;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Site detayı alınırken hata: {ex.Message}", ex);
            }
        }

        // Yeni site oluştur
        public async Task<SiteDto> CreateSiteAsync(SiteDto siteDto)
        {
            // DTO'yu entity'e dönüştür
            var site = _mapper.Map<TAppSite>(siteDto);
            
            // Oluşturma bilgilerini doldur
            site.Createddate = DateTime.Now;
            
            // Repository üzerinden kaydet
            var createdSite = await _siteRepository.AddAsync(site);
            
            // Kaydedilen entity'yi DTO'ya dönüştür ve geri döndür
            return _mapper.Map<SiteDto>(createdSite);
        }

        // Site bilgilerini güncelle
        public async Task<SiteDto> UpdateSiteAsync(SiteDto siteDto)
        {
            // Güncellenecek site ID kontrol et
            if (!siteDto.Id.HasValue)
                throw new ArgumentException("Site ID is required for update operation");
            
            // Mevcut site verilerini getir
            var existingSite = await _siteRepository.GetByIdAsync(siteDto.Id.Value);
            if (existingSite == null)
                throw new KeyNotFoundException($"Site with ID {siteDto.Id.Value} not found");
            
            // DTO'dan entity'ye dönüştür, ancak bazı alanları koru
            var site = _mapper.Map<SiteDto, TAppSite>(siteDto, existingSite);
            
            // Güncelleme bilgilerini doldur
            site.Modifieddate = DateTime.Now;
            
            // Repository üzerinden güncelle
            var updatedSite = await _siteRepository.UpdateAsync(site);
            
            // Güncellenen entity'yi DTO'ya dönüştür ve geri döndür
            return _mapper.Map<SiteDto>(updatedSite);
        }

        // Site sil (soft delete)
        public async Task DeleteSiteAsync(int id)
        {
            // Soft delete kullanarak siteyi sil
            await _siteRepository.SoftDeleteAsync(id);
        }

        // Site şablonlarını listele
        public async Task<IEnumerable<SiteListDto>> GetSiteTemplatesAsync()
        {
            // Şablon olarak işaretlenmiş siteleri getir
            var templates = await _siteRepository.GetTemplatesAsync();
            
            // DTO'ya dönüştür
            var templateDtos = _mapper.Map<IEnumerable<SiteListDto>>(templates);
            
            // Tema adlarını doldur
            foreach (var template in templateDtos)
            {
                if (template.ThemeId > 0) // Nullable kontrolü yerine değer kontrolü yapıyoruz
                {
                    var theme = await _themeRepository.GetByIdAsync(template.ThemeId);
                    if (theme != null)
                    {
                        template.ThemeName = theme.Name;
                    }
                }
            }
            
            return templateDtos;
        }

        // Alan adına göre site bilgisi getir
        public async Task<SiteDetailDto?> GetSiteByDomainAsync(string domain)
        {
            // Alan adına göre site bilgisini getir
            var site = await _siteRepository.GetByDomainAsync(domain);
            if (site == null)
                return null;
                
            // DTO'ya dönüştür
            return await GetSiteByIdAsync(site.Id);
        }

        // Aktif (yayında) olan siteleri listele
        public async Task<IEnumerable<SiteListDto>> GetActiveSitesAsync()
        {
            // Aktif siteleri getir
            var sites = await _siteRepository.GetActiveSitesAsync();
            
            // DTO'ya dönüştür
            var siteDtos = _mapper.Map<IEnumerable<SiteListDto>>(sites);
            
            // Tema adlarını doldur
            foreach (var site in siteDtos)
            {
                if (site.ThemeId > 0) // Nullable kontrolü yerine değer kontrolü yapıyoruz
                {
                    var theme = await _themeRepository.GetByIdAsync(site.ThemeId);
                    if (theme != null)
                    {
                        site.ThemeName = theme.Name;
                    }
                }
            }
            
            return siteDtos;
        }

        // Sayfalı ve filtrelenmiş site listesi döndür
        public async Task<(IEnumerable<SiteListDto> Items, int TotalCount)> GetPagedSitesAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true)
        {
            // Sayfalı site listesini getir
            var result = await _siteRepository.GetPagedSiteListAsync(pageNumber, pageSize, searchTerm, sortBy, ascending);
            
            // DTO'ya dönüştür
            var siteDtos = _mapper.Map<IEnumerable<SiteListDto>>(result.Items);
            
            // Tema adlarını doldur
            foreach (var site in siteDtos)
            {
                if (site.ThemeId > 0) // Nullable kontrolü yerine değer kontrolü yapıyoruz
                {
                    var theme = await _themeRepository.GetByIdAsync(site.ThemeId);
                    if (theme != null)
                    {
                        site.ThemeName = theme.Name;
                    }
                }
            }
            
            return (siteDtos, result.TotalCount);
        }

        // Alan adının benzersiz olup olmadığını kontrol et
        public async Task<bool> IsDomainUniqueAsync(string domain, int? excludeSiteId = null)
        {
            return await _siteRepository.IsDomainUniqueAsync(domain, excludeSiteId);
        }
    }
} 