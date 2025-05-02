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
        private readonly IRepository<TAppSitepage> _pageRepository;
        private readonly IRepository<TAppSite> _siteRepository; 
        private readonly IRepository<TAppThemecomponent> _themeComponentRepository;
        private readonly IRepository<TAppSitecomponentdata> _siteComponentDataRepository;
        private readonly IMapper _mapper;

        public PageService(
            IRepository<TAppSitepage> pageRepository,
            IRepository<TAppSite> siteRepository,
            IRepository<TAppThemecomponent> themeComponentRepository,
            IRepository<TAppSitecomponentdata> siteComponentDataRepository,
            IMapper mapper)
        {   
            _pageRepository = pageRepository;
            _siteRepository = siteRepository;
            _themeComponentRepository = themeComponentRepository;
            _siteComponentDataRepository = siteComponentDataRepository;
            _mapper = mapper;
        }

        
        /// Belirtilen siteye ait tüm aktif sayfaları listeler.
        public async Task<IEnumerable<PageListDto>> GetPagesBySiteIdAsync(int siteId)
        {
            try
            {
                var pages = await _pageRepository.Query()
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

        /// Belirtilen ID'ye sahip aktif sayfayı detaylarıyla getirir (TAppSitepage).
        public async Task<PageDetailDto?> GetPageByIdAsync(int id)
        {
            try
            {
                var page = await _pageRepository.Query()
                    .FirstOrDefaultAsync(p => p.Id == id && p.Isdeleted == 0);
                    
                return page == null ? null : _mapper.Map<PageDetailDto>(page);
            }
            catch (Exception ex)
            {

                throw new InvalidOperationException($"Sayfa detayı getirilirken hata oluştu (ID: {id}).", ex);
            }
        }

        /// Yeni bir site sayfası oluşturur ve ilişkili bileşen verilerini ekler.
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
                page.Isdeleted = 0;
                page.Createddate = DateTime.UtcNow;
                // page.Createduser = GetCurrentUserId(); // TODO

                // Sayfa kaydını oluştur
                var createdPage = await _pageRepository.AddAsync(page);

                // Sayfanın şablonuna/temasına göre bileşen verilerini oluştur
                int? themeIdToUse = null;
                if (createdPage.Templateid.HasValue && createdPage.Templateid > 0)
                {
                    // Sayfanın kendi şablonu varsa, o şablonun temasını bul
                    var templateSite = await _siteRepository.GetByIdAsync(createdPage.Templateid.Value);
                    if (templateSite != null && templateSite.Themeid > 0)
                    {
                        themeIdToUse = templateSite.Themeid;
                    }
                }
                else
                {
                     // Sayfanın şablonu yoksa, doğrudan bağlı olduğu sitenin temasını bul
                    var parentSite = await _siteRepository.GetByIdAsync(createdPage.Siteid);
                     if (parentSite != null && parentSite.Themeid > 0)
                    {
                        themeIdToUse = parentSite.Themeid;
                    }
                }

                if (themeIdToUse.HasValue)
                {
                    var themeComponents = await _themeComponentRepository.Query()
                        .Where(tc => tc.Themeid == themeIdToUse.Value && tc.Isdeleted == 0)
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
                                Data = "{}", // Başlangıç verisi (örn: boş JSON)
                                Isdeleted = 0,
                                Createddate = DateTime.UtcNow,
                                // Createduser = GetCurrentUserId() // TODO
                            };
                            siteComponentDataList.Add(siteComponentData);
                        }
                        await _siteComponentDataRepository.AddRangeAsync(siteComponentDataList);
                    }
                }

                return _mapper.Map<PageDto>(createdPage);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Sayfa oluşturulurken veritabanı hatası oluştu.", ex);
            }
            catch (Exception ex)
            {
                 if (ex is ArgumentException || ex is InvalidOperationException) throw;
                throw new InvalidOperationException("Sayfa oluşturulurken beklenmedik bir hata oluştu.", ex);
            }
        }

        /// Mevcut bir site sayfasının temel bilgilerini günceller.
        public async Task<PageDto> UpdatePageAsync(PageDto pageDto)
        {
            if (pageDto?.Id == null || pageDto.Id <= 0)
                throw new ArgumentNullException(nameof(pageDto), "Güncelleme için geçerli bir sayfa ID'si gereklidir.");

            try
            {
                var existingPage = await _pageRepository.GetByIdAsync(pageDto.Id.Value);
                if (existingPage == null || existingPage.Isdeleted == 1)
                    throw new KeyNotFoundException($"Güncellenecek sayfa bulunamadı veya silinmiş: ID {pageDto.Id.Value}");


                var originalIsDeleted = existingPage.Isdeleted;
                var originalCreatedDate = existingPage.Createddate;
                var originalCreatedUser = existingPage.Createduser;
                var originalSiteId = existingPage.Siteid; 

                _mapper.Map(pageDto, existingPage);

                existingPage.Isdeleted = originalIsDeleted;
                existingPage.Createddate = originalCreatedDate;
                existingPage.Createduser = originalCreatedUser;
                existingPage.Siteid = originalSiteId;

                existingPage.Modifieddate = DateTime.UtcNow;
                // existingPage.Modifieduser = GetCurrentUserId(); // TODO

                await _pageRepository.UpdateAsync(existingPage);
                return _mapper.Map<PageDto>(existingPage);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Sayfa güncellenirken veritabanı hatası oluştu (ID: {pageDto.Id.Value}).", ex);
            }
             catch (Exception ex)
            {
                 if (ex is KeyNotFoundException || ex is InvalidOperationException) throw;
                throw new InvalidOperationException($"Sayfa güncellenirken beklenmedik bir hata oluştu (ID: {pageDto.Id.Value}).", ex);
            }
        }

        /// Belirtilen ID'ye sahip sayfayı pasif hale getirir (soft delete).
        public async Task DeletePageAsync(int id)
        {
            try
            {
                await _pageRepository.SoftDeleteAsync(id); 
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Sayfa silinirken bir hata oluştu (ID: {id}).", ex);
            }
        }
    }
} 