using AutoMapper;
using Microsoft.EntityFrameworkCore;
using new_cms.Application.DTOs.SitemapDTOs;
using new_cms.Application.Interfaces;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace new_cms.Application.Services
{
    /// Site haritası (TAppSitemap) yönetimi ile ilgili işlemleri gerçekleştiren servis sınıfı
    public class SitemapService : ISitemapService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IIdGeneratorService _idGenerator;

        public SitemapService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IIdGeneratorService idGenerator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _idGenerator = idGenerator;
        }

        /// Sayfalı ve filtrelenmiş site haritası listesi getirir
        public async Task<(IEnumerable<SitemapListDto> Items, int TotalCount)> GetPagedSitemapsAsync(
            int pageNumber, 
            int pageSize, 
            int? siteId = null,
            string? domain = null,
            string? lang = null,
            int? column1 = null,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true)
        {
            try
            {
                var query = _unitOfWork.Repository<TAppSitemap>()
                    .Query()
                    .Where(s => s.Isdeleted == 0); // Soft delete filtresi

                // Site ID filtresi
                if (siteId.HasValue)
                {
                    query = query.Where(s => s.Siteid == siteId.Value);
                }

                // Domain filtresi
                if (!string.IsNullOrWhiteSpace(domain))
                {
                    query = query.Where(s => s.Domain != null && s.Domain.Contains(domain));
                }

                // Dil filtresi
                if (!string.IsNullOrWhiteSpace(lang))
                {
                    query = query.Where(s => s.Lang != null && s.Lang.Contains(lang));
                }

                // İçerik tipi filtresi
                if (column1.HasValue)
                {
                    query = query.Where(s => s.Column1 == column1.Value);
                }

                // Arama terimi filtreleme
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(s => 
                        (s.Url != null && s.Url.Contains(searchTerm)) ||
                        (s.Domain != null && s.Domain.Contains(searchTerm)) ||
                        (s.Column2 != null && s.Column2.Contains(searchTerm))
                    );
                }

                // Sıralama uygulama
                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    query = sortBy.ToLowerInvariant() switch
                    {
                        "id" => ascending ? query.OrderBy(s => s.Id) : query.OrderByDescending(s => s.Id),
                        "url" => ascending ? query.OrderBy(s => s.Url) : query.OrderByDescending(s => s.Url),
                        "domain" => ascending ? query.OrderBy(s => s.Domain) : query.OrderByDescending(s => s.Domain),
                        "lang" => ascending ? query.OrderBy(s => s.Lang) : query.OrderByDescending(s => s.Lang),
                        "active" => ascending ? query.OrderBy(s => s.Active) : query.OrderByDescending(s => s.Active),
                        "createddate" => ascending ? query.OrderBy(s => s.Createddate) : query.OrderByDescending(s => s.Createddate),
                        _ => ascending ? query.OrderBy(s => s.Id) : query.OrderByDescending(s => s.Id)
                    };
                }
                else
                {
                    // Varsayılan sıralama: ID'ye göre
                    query = ascending ? query.OrderBy(s => s.Id) : query.OrderByDescending(s => s.Id);
                }

                // Toplam kayıt sayısını al
                var totalCount = await query.CountAsync();

                // Sayfalama uygula
                var items = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var mappedItems = _mapper.Map<IEnumerable<SitemapListDto>>(items);

                return (mappedItems, totalCount);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Site haritası kayıtları getirilirken bir hata oluştu: {ex.Message}", ex);
            }
        }

        /// Belirtilen ID'ye sahip site haritası kaydını getirir
        public async Task<SitemapDto?> GetSitemapByIdAsync(int id)
        {
            try
            {
                var sitemap = await _unitOfWork.Repository<TAppSitemap>()
                    .Query()
                    .Where(s => s.Id == id && s.Isdeleted == 0)
                    .FirstOrDefaultAsync();

                return sitemap != null ? _mapper.Map<SitemapDto>(sitemap) : null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Site haritası kaydı getirilirken bir hata oluştu (ID: {id}): {ex.Message}", ex);
            }
        }

        /// Sistemdeki tüm aktif site haritası kayıtlarını getirir
        public async Task<IEnumerable<SitemapListDto>> GetActiveSitemapsAsync()
        {
            try
            {
                var sitemaps = await _unitOfWork.Repository<TAppSitemap>()
                    .Query()
                    .Where(s => s.Isdeleted == 0 && s.Active == 1)
                    .OrderBy(s => s.Url)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<SitemapListDto>>(sitemaps);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Aktif site haritası kayıtları getirilirken bir hata oluştu: {ex.Message}", ex);
            }
        }

        /// Belirtilen site'ye ait site haritası kayıtlarını getirir
        public async Task<IEnumerable<SitemapListDto>> GetSitemapsBySiteIdAsync(int siteId)
        {
            try
            {
                var sitemaps = await _unitOfWork.Repository<TAppSitemap>()
                    .Query()
                    .Where(s => s.Siteid == siteId && s.Isdeleted == 0)
                    .OrderBy(s => s.Url)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<SitemapListDto>>(sitemaps);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Site'ye ait site haritası kayıtları getirilirken bir hata oluştu (Site ID: {siteId}): {ex.Message}", ex);
            }
        }

        /// Belirtilen domain'e ait site haritası kayıtlarını getirir
        public async Task<IEnumerable<SitemapListDto>> GetSitemapsByDomainAsync(string domain)
        {
            try
            {
                var sitemaps = await _unitOfWork.Repository<TAppSitemap>()
                    .Query()
                    .Where(s => s.Domain == domain && s.Isdeleted == 0)
                    .OrderBy(s => s.Url)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<SitemapListDto>>(sitemaps);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Domain'e ait site haritası kayıtları getirilirken bir hata oluştu (Domain: {domain}): {ex.Message}", ex);
            }
        }

        /// Belirtilen dile ait site haritası kayıtlarını getirir
        public async Task<IEnumerable<SitemapListDto>> GetSitemapsByLangAsync(string lang)
        {
            try
            {
                var sitemaps = await _unitOfWork.Repository<TAppSitemap>()
                    .Query()
                    .Where(s => s.Lang == lang && s.Isdeleted == 0)
                    .OrderBy(s => s.Url)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<SitemapListDto>>(sitemaps);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Dile ait site haritası kayıtları getirilirken bir hata oluştu (Lang: {lang}): {ex.Message}", ex);
            }
        }

        /// Belirtilen URL için site haritası kaydını getirir
        public async Task<SitemapDto?> GetSitemapByUrlAsync(string url)
        {
            try
            {
                var sitemap = await _unitOfWork.Repository<TAppSitemap>()
                    .Query()
                    .Where(s => s.Url == url && s.Isdeleted == 0)
                    .FirstOrDefaultAsync();

                return sitemap != null ? _mapper.Map<SitemapDto>(sitemap) : null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"URL'e ait site haritası kaydı getirilirken bir hata oluştu (URL: {url}): {ex.Message}", ex);
            }
        }

        /// Yeni bir site haritası kaydı oluşturur
        public async Task<SitemapDto> CreateSitemapAsync(SitemapDto sitemapDto)
        {
            if (sitemapDto == null)
                throw new ArgumentNullException(nameof(sitemapDto));
            if (string.IsNullOrWhiteSpace(sitemapDto.Url))
                throw new ArgumentException("URL alanı boş olamaz.", nameof(sitemapDto.Url));

            try
            {
                // Aynı URL'nin olup olmadığını kontrol et
                var existingSitemap = await _unitOfWork.Repository<TAppSitemap>()
                    .Query()
                    .Where(s => s.Url == sitemapDto.Url && s.Isdeleted == 0)
                    .FirstOrDefaultAsync();

                if (existingSitemap != null)
                {
                    throw new InvalidOperationException($"Bu URL zaten mevcut: {sitemapDto.Url}");
                }

                var sitemap = _mapper.Map<TAppSitemap>(sitemapDto);
                
                sitemap.Id = await _idGenerator.GenerateNextIdAsync<TAppSitemap>();
                sitemap.Isdeleted = 0;
                sitemap.Createddate = DateTime.UtcNow;
                // sitemap.Createduser = GetCurrentUserId();

                var createdSitemap = await _unitOfWork.Repository<TAppSitemap>().AddAsync(sitemap);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<SitemapDto>(createdSitemap);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException || ex is ArgumentNullException || ex is InvalidOperationException) throw;
                throw new InvalidOperationException($"Site haritası kaydı oluşturulurken beklenmedik bir hata oluştu: {ex.Message}", ex);
            }
        }

        /// Mevcut bir site haritası kaydını günceller
        public async Task<SitemapDto> UpdateSitemapAsync(SitemapDto sitemapDto)
        {
            if (sitemapDto?.Id == null)
                throw new ArgumentNullException(nameof(sitemapDto), "Güncellenecek site haritası DTO'su veya ID'si null olamaz.");
            if (sitemapDto.Id <= 0)
                throw new ArgumentException("Güncelleme için geçerli bir site haritası ID'si gereklidir.", nameof(sitemapDto.Id));
            if (string.IsNullOrWhiteSpace(sitemapDto.Url))
                throw new ArgumentException("URL alanı boş olamaz.", nameof(sitemapDto.Url));

            try
            {
                var existingSitemap = await _unitOfWork.Repository<TAppSitemap>().GetByIdAsync(sitemapDto.Id.Value);

                if (existingSitemap == null || existingSitemap.Isdeleted == 1)
                {
                    throw new KeyNotFoundException($"Güncellenecek site haritası kaydı bulunamadı (ID: {sitemapDto.Id}).");
                }

                // Aynı URL'nin başka bir kayıtta olup olmadığını kontrol et
                var duplicateUrl = await _unitOfWork.Repository<TAppSitemap>()
                    .Query()
                    .Where(s => s.Url == sitemapDto.Url && s.Id != sitemapDto.Id && s.Isdeleted == 0)
                    .FirstOrDefaultAsync();

                if (duplicateUrl != null)
                {
                    throw new InvalidOperationException($"Bu URL başka bir kayıtta zaten mevcut: {sitemapDto.Url}");
                }

                // Sistem alanlarını koru
                var originalIsDeleted = existingSitemap.Isdeleted;
                var originalCreatedDate = existingSitemap.Createddate;
                var originalCreatedUser = existingSitemap.Createduser;

                _mapper.Map(sitemapDto, existingSitemap);
                
                existingSitemap.Isdeleted = originalIsDeleted;
                existingSitemap.Createddate = originalCreatedDate;
                existingSitemap.Createduser = originalCreatedUser;
                existingSitemap.Modifieddate = DateTime.UtcNow;
                // existingSitemap.Modifieduser = GetCurrentUserId();

                await _unitOfWork.Repository<TAppSitemap>().UpdateAsync(existingSitemap);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<SitemapDto>(existingSitemap);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException || ex is ArgumentNullException || ex is KeyNotFoundException || ex is InvalidOperationException) throw;
                throw new InvalidOperationException($"Site haritası kaydı güncellenirken beklenmedik bir hata oluştu: {ex.Message}", ex);
            }
        }

        /// Site haritası kaydını siler (soft delete)
        public async Task DeleteSitemapAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Geçerli bir site haritası ID'si gereklidir.", nameof(id));

            try
            {
                await _unitOfWork.Repository<TAppSitemap>().SoftDeleteAsync(id);
                await _unitOfWork.CompleteAsync();
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException($"Silinecek site haritası kaydı bulunamadı veya zaten silinmiş: ID {id}", ex);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException || ex is KeyNotFoundException) throw;
                throw new InvalidOperationException($"Site haritası kaydı silinirken beklenmedik bir hata oluştu: {ex.Message}", ex);
            }
        }

        /// Belirtilen içerik tipi (Column1) için site haritası kayıtlarını getirir
        public async Task<IEnumerable<SitemapListDto>> GetSitemapsByContentTypeAsync(int column1)
        {
            try
            {
                var sitemaps = await _unitOfWork.Repository<TAppSitemap>()
                    .Query()
                    .Where(s => s.Column1 == column1 && s.Isdeleted == 0)
                    .OrderBy(s => s.Url)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<SitemapListDto>>(sitemaps);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"İçerik tipine ait site haritası kayıtları getirilirken bir hata oluştu (Type: {column1}): {ex.Message}", ex);
            }
        }
    }
} 