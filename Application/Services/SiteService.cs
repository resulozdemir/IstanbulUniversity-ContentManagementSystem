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
    /// Site (TAppSite) varlıkları ile ilgili işlemleri gerçekleştiren servis sınıfı.
    public class SiteService : ISiteService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        private readonly ISiteDomainService _siteDomainService;
        
        private readonly IMapper _mapper;

        /// SiteService sınıfının yeni bir örneğini başlatır.
        public SiteService(
            IUnitOfWork unitOfWork, 
            ISiteDomainService siteDomainService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _siteDomainService = siteDomainService;
            _mapper = mapper;
        }

        /// Tüm aktif siteleri listeler.
        public async Task<IEnumerable<SiteListDto>> GetAllSitesAsync()
        {
            try
            {
                var sites = await _unitOfWork.Repository<TAppSite>().Query()
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

        /// Belirtilen ID'ye sahip aktif site detaylarını getirir.
        public async Task<SiteDetailDto?> GetSiteByIdAsync(int id)
        {
            try
            {
                var site = await _unitOfWork.Repository<TAppSite>().Query()
                    .Include(s => s.TAppSitedomains.Where(d => d.Isdeleted == 0))  
                    .Include(s => s.TAppSitepages.Where(p => p.Isdeleted == 0)) 
                    .Include(s => s.TAppNews.Where(n => n.Isdeleted == 0))      
                    .Include(s => s.TAppEvents.Where(e => e.Isdeleted == 0))    
                    .Include(s => s.TAppSitecomponentdata.Where(c => c.Isdeleted == 0)) 
                    .FirstOrDefaultAsync(s => s.Id == id && s.Isdeleted == 0); 

                if (site == null)
                    return null; 

                var siteDto = _mapper.Map<SiteDetailDto>(site);

                if (site.Themeid > 0)
                {
                    var theme = await _unitOfWork.Repository<TAppTheme>().GetByIdAsync(site.Themeid);
                    if (theme != null && theme.Isdeleted == 0)
                    {
                        siteDto.ThemeName = theme.Name;
                    }
                }

                siteDto.Domains = _mapper.Map<List<SiteDomainDto>>(site.TAppSitedomains);

                return siteDto;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Site detayı alınırken hata (ID: {id}): {ex.Message}", ex);
            }
        }

        /// Yeni bir site oluşturur ve ilişkili birincil alan adını kaydeder.
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

            try
            { 
                var site = _mapper.Map<TAppSite>(siteDto);
 
                site.Isdeleted = 0;                      
                site.Ispublish = 0;                  
                site.Createddate = DateTime.UtcNow;    
                // site.Createduser = GetCurrentUserId(); 
 
                var createdSite = await _unitOfWork.Repository<TAppSite>().AddAsync(site);
 
                var siteDomainDto = new SiteDomainDto
                {
                    SiteId = createdSite.Id, 
                    Domain = createdSite.Domain ?? string.Empty, 
                    Language = createdSite.Language ?? "tr", 
                    AnalyticId = createdSite.Analyticid,
                    GoogleSiteVerification = createdSite.Googlesiteverification,
                    Key = createdSite.Domain?.Replace(".","_") ?? Guid.NewGuid().ToString() 
                };
 
                await _siteDomainService.CreateDomainAsync(siteDomainDto); 

                await _unitOfWork.CompleteAsync();

                return _mapper.Map<SiteDto>(createdSite);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Site oluşturulurken veritabanı hatası: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
             catch (Exception ex)
            {
                throw new InvalidOperationException($"Site oluşturulurken beklenmedik bir hata: {ex.Message}", ex);
            }
        }

        /// Mevcut bir sitenin bilgilerini günceller.
        public async Task<SiteDto> UpdateSiteAsync(SiteDto siteDto)
        {
            if (!siteDto.Id.HasValue || siteDto.Id <= 0)
                throw new ArgumentException("Güncelleme için geçerli bir Site ID gereklidir.", nameof(siteDto.Id));

            try
            {
                var existingSite = await _unitOfWork.Repository<TAppSite>().GetByIdAsync(siteDto.Id.Value);

                if (existingSite == null || existingSite.Isdeleted == 1)
                    throw new KeyNotFoundException($"Site bulunamadı veya silinmiş: ID {siteDto.Id.Value}");

                // AutoMapper'ın üzerine yazmaması gereken alanları saklama
                var originalIsDeleted = existingSite.Isdeleted;
                var originalCreatedDate = existingSite.Createddate;
                var originalCreatedUser = existingSite.Createduser; 

                _mapper.Map(siteDto, existingSite);

                existingSite.Isdeleted = originalIsDeleted;
                existingSite.Createddate = originalCreatedDate;
                existingSite.Createduser = originalCreatedUser; 

                existingSite.Modifieddate = DateTime.UtcNow;
                // existingSite.Modifieduser = GetCurrentUserId(); 

                await _unitOfWork.Repository<TAppSite>().UpdateAsync(existingSite); 

                await _unitOfWork.CompleteAsync();

                return _mapper.Map<SiteDto>(existingSite);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Site güncellenirken veritabanı hatası (ID: {siteDto.Id.Value}): {ex.InnerException?.Message ?? ex.Message}", ex);
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException || ex is ArgumentException) throw;  
                throw new InvalidOperationException($"Site güncellenirken beklenmedik bir hata (ID: {siteDto.Id.Value}): {ex.Message}", ex);
            }
        }

        /// Belirtilen ID'ye sahip siteyi pasif hale getirir (soft delete).
        public async Task DeleteSiteAsync(int id)
        {
             try
            { 
                await _unitOfWork.Repository<TAppSite>().SoftDeleteAsync(id);

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex) 
            {
                throw new InvalidOperationException($"Site silinirken hata (ID: {id}): {ex.Message}", ex);
            }
        }

        /// Şablon olarak işaretlenmiş aktif siteleri listeler.
        public async Task<IEnumerable<SiteListDto>> GetSiteTemplatesAsync()
        {
            try
            {
                var templates = await _unitOfWork.Repository<TAppSite>().Query()
                    .Where(s => s.Istemplate == 1 && s.Isdeleted == 0) // Şablon ve aktif olanlar
                    .ToListAsync();

                var templateDtos = _mapper.Map<IEnumerable<SiteListDto>>(templates);
                return templateDtos;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Site şablonları listelenirken hata: {ex.Message}", ex);
            }
        }

        /// Belirtilen alan adına (domain) göre site detaylarını getirir.
        public async Task<SiteDetailDto?> GetSiteByDomainAsync(string domain)
        {
            try
            {
                var domainDto = await _siteDomainService.GetByDomainAsync(domain);
                
                if (domainDto?.SiteId == null) 
                    return null;

                return await GetSiteByIdAsync(domainDto.SiteId);
            }
             catch (Exception ex) 
            {
                throw new InvalidOperationException($"Domain '{domain}' için site getirilirken hata: {ex.Message}", ex);
            }
        }

        /// Yayında (published) olan aktif siteleri listeler.
        public async Task<IEnumerable<SiteListDto>> GetPublishedSitesAsync()
        {
            try
            {
                var sites = await _unitOfWork.Repository<TAppSite>().Query()
                    .Where(s => s.Ispublish == 1 && s.Isdeleted == 0) // Yayında ve aktif olanlar
                    .ToListAsync();

                var siteDtos = _mapper.Map<IEnumerable<SiteListDto>>(sites);
                return siteDtos;
            }
             catch (Exception ex)
            {
                throw new InvalidOperationException($"Yayındaki siteler listelenirken hata: {ex.Message}", ex);
            }
        }

        /// Sayfalanmış ve filtrelenmiş aktif site listesini döndürür.
        public async Task<(IEnumerable<SiteListDto> Items, int TotalCount)> GetPagedSitesAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,     // Arama için  
            string? sortBy = null,         // Sıralama alanı
            bool ascending = true)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10; 

            try
            {
                var query = _unitOfWork.Repository<TAppSite>().Query()
                               .Where(s => s.Isdeleted == 0); 

                // Arama terimlerine göre filtreleme
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(s => 
                        (s.Name != null && s.Name.Contains(searchTerm)) || 
                        (s.Domain != null && s.Domain.Contains(searchTerm))
                    );
                }
                
                // Sıralama uygulanması
                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    query = sortBy.ToLowerInvariant() switch 
                    {
                        "id" => ascending ? query.OrderBy(s => s.Id) : query.OrderByDescending(s => s.Id),
                        "name" => ascending ? query.OrderBy(s => s.Name) : query.OrderByDescending(s => s.Name),
                        "domain" => ascending ? query.OrderBy(s => s.Domain) : query.OrderByDescending(s => s.Domain),
                        "createddate" => ascending ? query.OrderBy(s => s.Createddate) : query.OrderByDescending(s => s.Createddate),
                        "modifieddate" => ascending ? query.OrderBy(s => s.Modifieddate) : query.OrderByDescending(s => s.Modifieddate),
                        "ispublish" => ascending ? query.OrderBy(s => s.Ispublish) : query.OrderByDescending(s => s.Ispublish),
                        _ => ascending ? query.OrderBy(s => s.Createddate) : query.OrderByDescending(s => s.Createddate)
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
                    .ToListAsync();

                var siteDtos = _mapper.Map<IEnumerable<SiteListDto>>(items);

                return (siteDtos, totalCount);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Sayfalanmış siteler getirilirken hata (Sayfa: {pageNumber}, Boyut: {pageSize}): {ex.Message}", ex);
            }
        }

        /// Belirli bir şablonu (template) kullanan aktif siteleri listeler.
        public async Task<IEnumerable<SiteListDto>> GetSitesByTemplateAsync(int templateId)
        {
            if (templateId <= 0) {
                 throw new ArgumentException("Geçerli bir şablon ID'si gereklidir.", nameof(templateId));
            }

            try
            {
                 // UnitOfWork üzerinden TAppSite repository'sine erişim
                var sites = await _unitOfWork.Repository<TAppSite>().Query()
                    .Where(s => s.Templateid == templateId    // Belirtilen şablonu kullanan
                                && s.Isdeleted == 0           // Aktif olan
                                && s.Istemplate == 0)         // Şablonun kendisi olmayan
                    .ToListAsync();

                var siteDtos = _mapper.Map<IEnumerable<SiteListDto>>(sites);
                return siteDtos;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Şablona göre siteler listelenirken hata oluştu (Template ID: {templateId}): {ex.Message}", ex);
            }
        }

        /// Belirtilen ID'ye sahip siteyi yayına alır  
        public async Task PublishSiteAsync(int siteId)
        {
            try
            {
                var site = await _unitOfWork.Repository<TAppSite>().GetByIdAsync(siteId);
                
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
                // site.Modifieduser = GetCurrentUserId(); // TODO: Kullanıcı ID'si eklenmeli

                await _unitOfWork.Repository<TAppSite>().UpdateAsync(site);
                
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Site yayınlanırken veritabanı hatası oluştu (ID: {siteId}): {ex.InnerException?.Message ?? ex.Message}", ex);
            }
            catch (Exception ex)
            {
                 if (ex is KeyNotFoundException) throw;
                throw new InvalidOperationException($"Site yayınlanırken beklenmedik bir hata oluştu (ID: {siteId}): {ex.Message}", ex);
            }
        }

        /// Belirtilen ID'ye sahip siteyi yayından kaldırır  
        public async Task UnpublishSiteAsync(int siteId)
        {
             try
            {
                var site = await _unitOfWork.Repository<TAppSite>().GetByIdAsync(siteId);

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
                // site.Modifieduser = GetCurrentUserId(); 

                await _unitOfWork.Repository<TAppSite>().UpdateAsync(site);
                
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Site yayından kaldırılırken veritabanı hatası oluştu (ID: {siteId}): {ex.InnerException?.Message ?? ex.Message}", ex);
            }
            catch (Exception ex)
            {
                 if (ex is KeyNotFoundException) throw;
                throw new InvalidOperationException($"Site yayından kaldırılırken beklenmedik bir hata oluştu (ID: {siteId}): {ex.Message}", ex);
            }
        }
    }
} 