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
        private readonly ITemplateService _templateService;

        /// SiteService sınıfının yeni bir örneğini başlatır.
        public SiteService(
            IUnitOfWork unitOfWork, 
            ISiteDomainService siteDomainService,
            IMapper mapper,
            ITemplateService templateService)
        {
            _unitOfWork = unitOfWork;
            _siteDomainService = siteDomainService;
            _mapper = mapper;
            _templateService = templateService;
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
                        siteDto.ThemeName = theme.Name ?? string.Empty;
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
            // Domain adı boş değilse benzersizlik kontrolü yap
            if (!string.IsNullOrWhiteSpace(siteDto.Domain) && !await _siteDomainService.IsDomainUniqueAsync(siteDto.Domain))
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
                    Domain = string.IsNullOrWhiteSpace(createdSite.Domain) ? string.Empty : createdSite.Domain, 
                    Language = createdSite.Language ?? "tr", 
                    AnalyticId = createdSite.Analyticid,
                    GoogleSiteVerification = createdSite.Googlesiteverification, 
                    Key = !string.IsNullOrWhiteSpace(createdSite.Domain) ? createdSite.Domain.Replace(".","_") : Guid.NewGuid().ToString() 
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
            if (siteDto.Id == null || siteDto.Id.Value <= 0)
                throw new ArgumentException("Güncelleme için geçerli bir Site ID gereklidir.", nameof(siteDto.Id));

            try
            {
                var existingSite = await _unitOfWork.Repository<TAppSite>().GetByIdAsync(siteDto.Id.Value);

                if (existingSite == null || existingSite.Isdeleted == 1)
                    throw new KeyNotFoundException($"Site bulunamadı veya silinmiş: ID {siteDto.Id}");

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
                throw new InvalidOperationException($"Site güncellenirken veritabanı hatası (ID: {siteDto.Id}): {ex.InnerException?.Message ?? ex.Message}", ex);
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException || ex is ArgumentException) throw;  
                throw new InvalidOperationException($"Site güncellenirken beklenmedik bir hata (ID: {siteDto.Id}): {ex.Message}", ex);
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

        /// Belirtilen bir şablonu kullanarak yeni bir site oluşturur ve şablon içeriğini kopyalar.
        public async Task<SiteDetailDto> CreateSiteFromTemplateAsync(SiteDto siteDto)
        {
            // 1. Kaynak şablonun ID'sini siteDto.TemplateId'den al.
            if (siteDto.TemplateId == null || siteDto.TemplateId.Value <= 0)
            {
                throw new ArgumentException("Yeni site oluşturmak için geçerli bir kaynak şablon ID'si (TemplateId) belirtilmelidir.");
            }
            int sourceTemplateId = siteDto.TemplateId.Value;

            var sourceTemplate = await _unitOfWork.Repository<TAppSite>().Query()
                .AsNoTracking() 
                .FirstOrDefaultAsync(s => s.Id == sourceTemplateId && s.Istemplate == 1 && s.Isdeleted == 0);
            
            if (sourceTemplate == null)
            {
                throw new KeyNotFoundException($"Kaynak şablon (ID: {sourceTemplateId}) bulunamadı, aktif değil veya bir şablon değil.");
            }

            if (siteDto.IsTemplate == 1) {
                 throw new ArgumentException("Şablondan oluşturulan bir site, kendisi şablon olarak işaretlenemez (IsTemplate 0 olmalıdır).");
            }

            var newSiteEntity = _mapper.Map<TAppSite>(siteDto);
            newSiteEntity.Id = 0; 
            newSiteEntity.Istemplate = 0; 
            newSiteEntity.Isdeleted = 0;
            newSiteEntity.Createddate = DateTime.UtcNow;
            newSiteEntity.Templateid = sourceTemplateId; 

            var newSiteThemeExists = await _unitOfWork.Repository<TAppTheme>().Query()
                                        .AnyAsync(t => t.Id == newSiteEntity.Themeid && t.Isdeleted == 0);
            if (!newSiteThemeExists)
            {
                throw new KeyNotFoundException($"Yeni site için belirtilen tema ID ({newSiteEntity.Themeid}) bulunamadı veya aktif değil.");
            }
            
            if (!string.IsNullOrWhiteSpace(newSiteEntity.Domain) && !await _siteDomainService.IsDomainUniqueAsync(newSiteEntity.Domain, null)) 
            {
                throw new InvalidOperationException($"'{newSiteEntity.Domain}' alan adı zaten kullanılıyor.");
            }

            var createdSiteEntity = await _unitOfWork.Repository<TAppSite>().AddAsync(newSiteEntity);
            await _unitOfWork.CompleteAsync(); 

            if (!string.IsNullOrWhiteSpace(createdSiteEntity.Domain))
            {
                var siteDomainDto = new SiteDomainDto
                {
                    SiteId = createdSiteEntity.Id,
                    Domain = createdSiteEntity.Domain,
                    Language = createdSiteEntity.Language ?? "tr",
                    AnalyticId = createdSiteEntity.Analyticid,
                    GoogleSiteVerification = createdSiteEntity.Googlesiteverification,
                    Key = createdSiteEntity.Domain.Replace(".", "_") 
                };
                await _siteDomainService.CreateDomainAsync(siteDomainDto);
                await _unitOfWork.CompleteAsync(); 
            }

            await _templateService.CopyTemplateContentToSiteAsync(sourceTemplateId, createdSiteEntity.Id);

            return await GetSiteByIdAsync(createdSiteEntity.Id) 
                   ?? throw new InvalidOperationException($"Şablondan oluşturulan site (ID: {createdSiteEntity.Id}) oluşturulduktan sonra bulunamadı.");
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