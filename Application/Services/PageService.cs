using AutoMapper;
using Microsoft.EntityFrameworkCore;
using new_cms.Application.DTOs.PageDTOs;
using new_cms.Application.Interfaces;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace new_cms.Application.Services
{
    
    /// Site sayfaları (TAppSitepage) yönetimi ile ilgili işlemleri gerçekleştiren servis sınıfı.
    public class PageService : IPageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IIdGeneratorService _idGenerator;
 
        /// PageService sınıfının yeni bir örneğini başlatır.  
        public PageService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IIdGeneratorService idGenerator)
        {   
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _idGenerator = idGenerator;
        }

        /// Sayfalı ve filtrelenmiş aktif sayfa listesini döndürür.
        public async Task<(IEnumerable<PageListDto> Items, int TotalCount)> GetPagedPagesAsync(
            int pageNumber, 
            int pageSize, 
            int? siteId = null, 
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true)
        { 
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;  

            try
            { 
                var query = _unitOfWork.Repository<TAppSitepage>().Query()
                    .Where(p => p.Isdeleted == 0);  
  
                if (siteId.HasValue && siteId.Value > 0)
                {
                    query = query.Where(p => p.Siteid == siteId.Value);
                }
  
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(p => 
                        (p.Name != null && p.Name.Contains(searchTerm))
                    );
                }
                 
                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    query = sortBy.ToLowerInvariant() switch 
                    {
                        "id" => ascending ? query.OrderBy(p => p.Id) : query.OrderByDescending(p => p.Id),
                        "name" => ascending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                        "createddate" => ascending ? query.OrderBy(p => p.Createddate) : query.OrderByDescending(p => p.Createddate),
                        "modifieddate" => ascending ? query.OrderBy(p => p.Modifieddate) : query.OrderByDescending(p => p.Modifieddate),
                        _ => ascending ? query.OrderBy(p => p.Id) : query.OrderByDescending(p => p.Id)
                    };
                }
                else
                {
                    query = query.OrderByDescending(p => p.Id);
                }
                
                var totalCount = await query.CountAsync();
                
                var items = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (_mapper.Map<IEnumerable<PageListDto>>(items), totalCount);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Sayfalanmış sayfalar listelenirken bir hata oluştu (Sayfa: {pageNumber}, Boyut: {pageSize}).", ex);
            }
        }

        /// Belirtilen siteye ait tüm aktif sayfaları listeler. 
        public async Task<IEnumerable<PageListDto>> GetPagesBySiteIdAsync(int siteId)
        {
            if (siteId <= 0) {
                 throw new ArgumentException("Geçerli bir Site ID'si gereklidir.", nameof(siteId));
            }

            try
            { 
                var pages = await _unitOfWork.Repository<TAppSitepage>().Query()
                    .Where(p => p.Siteid == siteId && p.Isdeleted == 0)
                    .OrderBy(p => p.Id)  
                    .ToListAsync();
                return _mapper.Map<IEnumerable<PageListDto>>(pages);
            }
            catch (Exception ex)
            { 
                throw new InvalidOperationException($"Siteye ait sayfalar listelenirken hata oluştu (Site ID: {siteId}).", ex);
            }
        }
 
        /// Belirtilen ID'ye sahip aktif sayfayı detaylarıyla getirir. 
        public async Task<PageDetailDto?> GetPageByIdAsync(int id)
        {
            if (id <= 0) {
                 throw new ArgumentException("Geçerli bir sayfa ID'si gereklidir.", nameof(id));
            }

            try
            { 
                var page = await _unitOfWork.Repository<TAppSitepage>().Query()
                    .FirstOrDefaultAsync(p => p.Id == id && p.Isdeleted == 0);  
                    
                return page == null ? null : _mapper.Map<PageDetailDto>(page);
            }
            catch (Exception ex)
            {
                // Hata yönetimi
                throw new InvalidOperationException($"Sayfa detayı getirilirken hata oluştu (ID: {id}).", ex);
            }
        }
 
        /// Yeni bir site sayfası oluşturur ve ilişkili tema bileşen verilerini ekler. 
        public async Task<PageDto> CreatePageAsync(PageDto pageDto)
        { 
            if (pageDto == null)
                throw new ArgumentNullException(nameof(pageDto));
            if (pageDto.SiteId <= 0)
                 throw new ArgumentException("Geçerli bir Site ID'si gereklidir.", nameof(pageDto.SiteId));
            if (string.IsNullOrWhiteSpace(pageDto.Name))
                 throw new ArgumentException("Sayfa adı boş olamaz.", nameof(pageDto.Name));

            try
            { 
                var page = _mapper.Map<TAppSitepage>(pageDto);
                 
                page.Id = await _idGenerator.GenerateNextIdAsync<TAppSitepage>();
                
                page.Isdeleted = 0;  
                page.Createddate = DateTime.UtcNow;  
                // page.Createduser = GetCurrentUserId();  
 
                var createdPage = await _unitOfWork.Repository<TAppSitepage>().AddAsync(page);

                // Sayfanın şablonuna veya doğrudan sitesine bağlı temayı bul
                int? themeIdToUse = null;
                if (createdPage.Templateid.HasValue && createdPage.Templateid > 0)
                {
                    // Sayfanın kendi şablonu varsa, o şablonun  temasını bul
                    var templateSite = await _unitOfWork.Repository<TAppSite>().GetByIdAsync(createdPage.Templateid.Value);
                    if (templateSite != null && templateSite.Themeid > 0 && templateSite.Isdeleted == 0) // Şablon site aktif olmalı
                    {
                        themeIdToUse = templateSite.Themeid;
                    }
                }
                else
                {
                     // Sayfanın şablonu yoksa, doğrudan bağlı olduğu sitenin temasını bul
                    var parentSite = await _unitOfWork.Repository<TAppSite>().GetByIdAsync(createdPage.Siteid);
                     if (parentSite != null && parentSite.Themeid > 0 && parentSite.Isdeleted == 0) 
                    {
                        themeIdToUse = parentSite.Themeid;
                    }
                }

                // Eğer kullanılacak bir tema bulunduysa, tema bileşenlerini al ve sayfa için bileşen verilerini oluştur
                if (themeIdToUse.HasValue)
                {
                    var themeComponents = await _unitOfWork.Repository<TAppThemecomponent>().Query()
                        .Where(tc => tc.Themeid == themeIdToUse.Value && tc.Isdeleted == 0) // Aktif tema bileşenleri
                        .ToListAsync();
                    
                    if (themeComponents.Any())
                    {
                        var siteComponentDataList = new List<TAppSitecomponentdata>();
                        foreach (var themeComponent in themeComponents)
                        {
                            // Her tema bileşeni için bir site bileşen verisi oluştur
                            var siteComponentData = new TAppSitecomponentdata
                            {
                                Siteid = createdPage.Siteid,           
                                Themecomponentid = themeComponent.Id,  
                                Data = "{}", // Başlangıç verisi (boş JSON veya varsayılan yapı)
                                Isdeleted = 0,  
                                Createddate = DateTime.UtcNow,
                                // Createduser = GetCurrentUserId()  
                            };
                            siteComponentDataList.Add(siteComponentData);
                        } 
                        await _unitOfWork.Repository<TAppSitecomponentdata>().AddRangeAsync(siteComponentDataList);
                    }
                }
 
                await _unitOfWork.CompleteAsync();
 
                return _mapper.Map<PageDto>(createdPage);
            }
            catch (DbUpdateException ex)
            { 
                throw new InvalidOperationException($"Sayfa oluşturulurken veritabanı hatası oluştu: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
            catch (Exception ex)
            { 
                 if (ex is ArgumentException || ex is ArgumentNullException || ex is InvalidOperationException) throw; // Zaten yukarıda ele alınanlar
                throw new InvalidOperationException($"Sayfa oluşturulurken beklenmedik bir hata oluştu: {ex.Message}", ex);
            }
        }
 
        /// Mevcut bir site sayfasının temel bilgilerini günceller. 
        public async Task<PageDto> UpdatePageAsync(PageDto pageDto)
        { 
            if (pageDto?.Id == null)
                 throw new ArgumentNullException(nameof(pageDto), "Güncellenecek sayfa DTO'su veya ID'si null olamaz.");
            if (pageDto.Id <= 0)
                throw new ArgumentException("Güncelleme için geçerli bir sayfa ID'si gereklidir.", nameof(pageDto.Id));
            if (string.IsNullOrWhiteSpace(pageDto.Name))
                 throw new ArgumentException("Sayfa adı boş olamaz.", nameof(pageDto.Name)); 

            try
            {
                var existingPage = await _unitOfWork.Repository<TAppSitepage>().GetByIdAsync(pageDto.Id.Value); 
                if (existingPage == null || existingPage.Isdeleted == 1)
                    throw new KeyNotFoundException($"Güncellenecek sayfa bulunamadı veya silinmiş: ID {pageDto.Id.Value}");
 
                var originalIsDeleted = existingPage.Isdeleted;
                var originalCreatedDate = existingPage.Createddate;
                var originalCreatedUser = existingPage.Createduser;
                var originalSiteId = existingPage.Siteid; 
                var originalTemplateId = existingPage.Templateid;  
 
                _mapper.Map(pageDto, existingPage);
 
                existingPage.Isdeleted = originalIsDeleted;
                existingPage.Createddate = originalCreatedDate;
                existingPage.Createduser = originalCreatedUser;
                existingPage.Siteid = originalSiteId;   
                existingPage.Templateid = originalTemplateId;   
 
                existingPage.Modifieddate = DateTime.UtcNow; 
 
                await _unitOfWork.Repository<TAppSitepage>().UpdateAsync(existingPage); 
                await _unitOfWork.CompleteAsync();
 
                return _mapper.Map<PageDto>(existingPage);
            }
            catch (DbUpdateException ex)
            { 
                throw new InvalidOperationException($"Sayfa güncellenirken veritabanı hatası oluştu (ID: {pageDto.Id.Value}): {ex.InnerException?.Message ?? ex.Message}", ex);
            }
             catch (Exception ex)
            { 
                if (ex is KeyNotFoundException || ex is ArgumentNullException || ex is ArgumentException || ex is InvalidOperationException) throw;
                throw new InvalidOperationException($"Sayfa güncellenirken beklenmedik bir hata oluştu (ID: {pageDto.Id.Value}): {ex.Message}", ex);
            }
        }
 
        /// Belirtilen ID'ye sahip sayfayı pasif hale getirir. 
        public async Task DeletePageAsync(int id)
        {
             if (id <= 0) {
                 throw new ArgumentException("Geçerli bir sayfa ID'si gereklidir.", nameof(id));
            }
            try
            {
                // UnitOfWork üzerinden TAppSitepage repository'sine erişim ve SoftDeleteAsync çağrısı
                // Bu metodun ilgili entity'yi bulup IsDeleted=1 ve ModifiedDate ayarladığını varsayıyoruz.
                await _unitOfWork.Repository<TAppSitepage>().SoftDeleteAsync(id);  
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex) 
            {
                throw new InvalidOperationException($"Sayfa silinirken bir hata oluştu (ID: {id}).", ex);
            }
        }

        /// Sistemdeki tüm sayfaları listeler.
        public async Task<IEnumerable<PageListDto>> GetAllPagesAsync()
        {
            try
            {
                var pages = await _unitOfWork.Repository<TAppSitepage>().Query()
                    .OrderBy(p => p.Id)
                    .ToListAsync();
                    
                return _mapper.Map<IEnumerable<PageListDto>>(pages);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Tüm sayfalar listelenirken bir hata oluştu.", ex);
            }
        }

        /// Sistemdeki tüm aktif sayfaları listeler.
        public async Task<IEnumerable<PageListDto>> GetActivePagesAsync()
        {
            try
            {
                var activePages = await _unitOfWork.Repository<TAppSitepage>().Query()
                    .Where(p => p.Isdeleted == 0)
                    .OrderBy(p => p.Id)
                    .ToListAsync();
                    
                return _mapper.Map<IEnumerable<PageListDto>>(activePages);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Aktif sayfalar listelenirken bir hata oluştu.", ex);
            }
        }
    }
} 