using AutoMapper;
using Microsoft.EntityFrameworkCore;
using new_cms.Application.DTOs.NewsDTOs;
using new_cms.Application.Interfaces;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace new_cms.Application.Services
{
    /// Haber (TAppNews) varlıkları ile ilgili işlemleri gerçekleştiren servis sınıfı.
    public class NewsService : INewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NewsService(
            IUnitOfWork unitOfWork,  
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<NewsListDto> Items, int TotalCount)> GetPagedNewsAsync(
            int pageNumber, int pageSize, int? siteId = null, string? searchTerm = null, string? sortBy = null, bool ascending = true)
        { 
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;  

            try
            { 
                var query = _unitOfWork.Repository<TAppNews>().Query().Where(n => n.Isdeleted == 0);  
 
                if (siteId.HasValue && siteId.Value > 0)  
                {
                    query = query.Where(n => n.Siteid == siteId.Value);
                }
 
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(n => 
                        (n.Header != null && n.Header.Contains(searchTerm)) || 
                        (n.Content != null && n.Content.Contains(searchTerm)) ||
                        (n.Tag != null && n.Tag.Contains(searchTerm))
                    );
                }
                 
                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    query = sortBy.ToLowerInvariant() switch 
                    {
                        "id" => ascending ? query.OrderBy(n => n.Id) : query.OrderByDescending(n => n.Id),
                        "header" => ascending ? query.OrderBy(n => n.Header) : query.OrderByDescending(n => n.Header),
                        "date" => ascending ? query.OrderBy(n => n.Ondate) : query.OrderByDescending(n => n.Ondate),
                        "createddate" => ascending ? query.OrderBy(n => n.Createddate) : query.OrderByDescending(n => n.Createddate), 
                        "modifieddate" => ascending ? query.OrderBy(n => n.Modifieddate) : query.OrderByDescending(n => n.Modifieddate), 
                        "ispublish" => ascending ? query.OrderBy(n => n.Ispublish) : query.OrderByDescending(n => n.Ispublish), 
                        "priorityorder" => ascending ? query.OrderBy(n => n.Priorityorder) : query.OrderByDescending(n => n.Priorityorder), 
                        _ => ascending ? query.OrderBy(n => n.Ondate) : query.OrderByDescending(n => n.Ondate) 
                    };
                }
                else
                {
                    // Varsayılan sıralama
                    query = ascending ? query.OrderBy(n => n.Ondate) : query.OrderByDescending(n => n.Ondate);
                }
                
                // Toplam kayıt sayısını al
                var totalCount = await query.CountAsync();

                // Sayfalama uygula
                var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                return (_mapper.Map<IEnumerable<NewsListDto>>(items), totalCount);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Haberler listelenirken bir hata oluştu.", ex);
            }
        }

        public async Task<NewsDto?> GetNewsByIdAsync(int id)
        {
            if (id <= 0) {
                 throw new ArgumentException("Geçerli bir haber ID'si gereklidir.", nameof(id));
            }

            try
            {
                var news = await _unitOfWork.Repository<TAppNews>().Query()
                                .FirstOrDefaultAsync(n => n.Id == id && n.Isdeleted == 0);
                
                return news == null ? null : _mapper.Map<NewsDto>(news);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Haber getirilirken bir hata oluştu (ID: {id}).", ex);
            }
        }

        public async Task<NewsDto> CreateNewsAsync(NewsDto newsDto)
        {
            if (newsDto == null) {
                 throw new ArgumentNullException(nameof(newsDto), "Oluşturulacak haber bilgileri boş olamaz.");
            }
            if (string.IsNullOrWhiteSpace(newsDto.Header)) {
                 throw new ArgumentException("Haber başlığı boş olamaz.", nameof(newsDto.Header));
            }
            if (newsDto.SiteId <= 0) { 
                 throw new ArgumentException("Geçerli bir Site ID'si gereklidir.", nameof(newsDto.SiteId));
            }

            try
            {
                var news = _mapper.Map<TAppNews>(newsDto);
                news.Isdeleted = 0; 
                news.Createddate = DateTime.UtcNow; 
                // news.Createduser = GetCurrentUserId(); // TODO: Aktif kullanıcı ID'si entegre edilmeli

                var createdNews = await _unitOfWork.Repository<TAppNews>().AddAsync(news);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<NewsDto>(createdNews);
            }
            catch (DbUpdateException ex) {
                throw new InvalidOperationException($"Haber oluşturulurken veritabanı hatası: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException || ex is ArgumentException) throw;
                throw new InvalidOperationException("Haber oluşturulurken beklenmedik bir hata oluştu.", ex);
            }
        }

        public async Task<NewsDto> UpdateNewsAsync(NewsDto newsDto)
        {
            // Girdi doğrulamaları
            if (newsDto?.Id == null || newsDto.Id <= 0)
                 throw new ArgumentNullException(nameof(newsDto), "Güncelleme için geçerli bir Haber ID'si gereklidir.");
            if (string.IsNullOrWhiteSpace(newsDto.Header)) {
                 throw new ArgumentException("Haber başlığı boş olamaz.", nameof(newsDto.Header));
            }
            if (newsDto.SiteId <= 0) { 
                 throw new ArgumentException("Geçerli bir Site ID'si gereklidir.", nameof(newsDto.SiteId));
            }

            try
            {
                var existingNews = await _unitOfWork.Repository<TAppNews>().GetByIdAsync(newsDto.Id.Value);
                
                if (existingNews == null || existingNews.Isdeleted == 1)
                    throw new KeyNotFoundException($"Güncellenecek haber bulunamadı veya silinmiş: ID {newsDto.Id.Value}");

                // AutoMapper'ın üzerine yazmaması gereken sistem tarafından yönetilen alanları sakla
                var originalIsDeleted = existingNews.Isdeleted;
                var originalCreatedDate = existingNews.Createddate;
                var originalCreatedUser = existingNews.Createduser;
                // var originalSiteId = existingNews.Siteid; // SiteID genellikle güncellenmez, gerekirse bu da korunmalı.

                _mapper.Map(newsDto, existingNews);

                existingNews.Isdeleted = originalIsDeleted;
                existingNews.Createddate = originalCreatedDate;
                existingNews.Createduser = originalCreatedUser;

                // Güncelleme bilgilerini ayarla
                existingNews.Modifieddate = DateTime.UtcNow;
                // existingNews.Modifieduser = GetCurrentUserId(); // TODO: Aktif kullanıcı ID'si eklenmeli

                await _unitOfWork.Repository<TAppNews>().UpdateAsync(existingNews);
                await _unitOfWork.CompleteAsync();
                
                return _mapper.Map<NewsDto>(existingNews);
            }
            catch (DbUpdateException ex) {
                 throw new InvalidOperationException($"Haber güncellenirken veritabanı hatası (ID: {newsDto.Id.Value}): {ex.InnerException?.Message ?? ex.Message}", ex);
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException || ex is ArgumentNullException || ex is ArgumentException) throw;
                throw new InvalidOperationException($"Haber güncellenirken beklenmedik bir hata oluştu (ID: {newsDto.Id.Value}).", ex);
            }
        }

        public async Task DeleteNewsAsync(int id)
        {
            if (id <= 0) {
                 throw new ArgumentException("Geçerli bir haber ID'si gereklidir.", nameof(id));
            }

            try
            {
                await _unitOfWork.Repository<TAppNews>().SoftDeleteAsync(id);
                await _unitOfWork.CompleteAsync();
            }
            catch (KeyNotFoundException ex) 
            {
                throw new KeyNotFoundException($"Silinecek haber bulunamadı veya zaten silinmiş: ID {id}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Haber silinirken bir hata oluştu (ID: {id}).", ex);
            }
        }

        /// Sistemdeki tüm haberleri listeler.
        public async Task<IEnumerable<NewsListDto>> GetAllNewsAsync()
        {
            try
            {
                var allNews = await _unitOfWork.Repository<TAppNews>().Query()
                    .OrderByDescending(n => n.Ondate)
                    .ToListAsync();
                    
                return _mapper.Map<IEnumerable<NewsListDto>>(allNews);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Tüm haberler listelenirken bir hata oluştu.", ex);
            }
        }

        /// Belirtilen siteye ait haberleri listeler.
        public async Task<IEnumerable<NewsListDto>> GetNewsBySiteIdAsync(int siteId)
        {
            if (siteId <= 0)
            {
                throw new ArgumentException("Geçerli bir Site ID'si gereklidir.", nameof(siteId));
            }

            try
            {
                var siteNews = await _unitOfWork.Repository<TAppNews>().Query()
                    .Where(n => n.Siteid == siteId)
                    .OrderByDescending(n => n.Ondate)
                    .ToListAsync();
                    
                return _mapper.Map<IEnumerable<NewsListDto>>(siteNews);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Site haberleri listelenirken hata oluştu (Site ID: {siteId}).", ex);
            }
        }

        /// Sistemdeki tüm aktif haberleri listeler.
        public async Task<IEnumerable<NewsListDto>> GetActiveNewsAsync()
        {
            try
            {
                var activeNews = await _unitOfWork.Repository<TAppNews>().Query()
                    .Where(n => n.Isdeleted == 0)
                    .OrderByDescending(n => n.Ondate)
                    .ToListAsync();
                    
                return _mapper.Map<IEnumerable<NewsListDto>>(activeNews);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Aktif haberler listelenirken bir hata oluştu.", ex);
            }
        }
    }
} 