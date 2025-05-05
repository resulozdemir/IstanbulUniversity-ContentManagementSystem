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
    /// Haber (News) işlemleri için servis implementasyonu.
    public class NewsService : INewsService
    {
        private readonly IRepository<TAppNews> _newsRepository;
        private readonly IMapper _mapper;

        public NewsService(
            IRepository<TAppNews> newsRepository,
            IMapper mapper)
        {
            _newsRepository = newsRepository;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<(IEnumerable<NewsListDto> Items, int TotalCount)> GetPagedNewsAsync(
            int pageNumber, int pageSize, int? siteId = null, string? searchTerm = null, string? sortBy = null, bool ascending = true)
        {
            try
            {
                // Silinmemiş haberleri sorgula
                var query = _newsRepository.Query().Where(n => n.Isdeleted == 0);

                // Site ID'sine göre filtrele
                if (siteId.HasValue)
                {
                    query = query.Where(n => n.Siteid == siteId.Value);
                }

                // Arama terimine göre filtrele (Başlık, İçerik, Etiket)
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(n => n.Header.Contains(searchTerm) || 
                                             (n.Content != null && n.Content.Contains(searchTerm)) ||
                                             (n.Tag != null && n.Tag.Contains(searchTerm)));
                }
                
                // Dinamik sıralama
                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    query = sortBy.ToLower() switch
                    {
                        "id" => ascending ? query.OrderBy(n => n.Id) : query.OrderByDescending(n => n.Id),
                        "header" => ascending ? query.OrderBy(n => n.Header) : query.OrderByDescending(n => n.Header),
                        "date" => ascending ? query.OrderBy(n => n.Ondate) : query.OrderByDescending(n => n.Ondate),
                        "created" => ascending ? query.OrderBy(n => n.Createddate) : query.OrderByDescending(n => n.Createddate),
                        "modified" => ascending ? query.OrderBy(n => n.Modifieddate) : query.OrderByDescending(n => n.Modifieddate),
                        "publish" => ascending ? query.OrderBy(n => n.Ispublish) : query.OrderByDescending(n => n.Ispublish),
                        "priority" => ascending ? query.OrderBy(n => n.Priorityorder) : query.OrderByDescending(n => n.Priorityorder),
                        _ => ascending ? query.OrderBy(n => n.Ondate) : query.OrderByDescending(n => n.Ondate) // Varsayılan tarih sıralaması
                    };
                }
                else
                {
                    // Varsayılan sıralama (sortBy belirtilmemişse)
                    query = ascending ? query.OrderBy(n => n.Ondate) : query.OrderByDescending(n => n.Ondate);
                }
                
                var totalCount = await query.CountAsync();

                var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                return (_mapper.Map<IEnumerable<NewsListDto>>(items), totalCount);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Haberler listelenirken bir hata oluştu.", ex);
            }
        }

        /// <inheritdoc />
        public async Task<NewsDto?> GetNewsByIdAsync(int id)
        {
            try
            {
                var news = await _newsRepository.Query().FirstOrDefaultAsync(n => n.Id == id && n.Isdeleted == 0);
                
                return news == null ? null : _mapper.Map<NewsDto>(news);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Haber getirilirken bir hata oluştu (ID: {id}).", ex);
            }
        }

        /// <inheritdoc />
        public async Task<NewsDto> CreateNewsAsync(NewsDto newsDto)
        {
            try
            {
                var news = _mapper.Map<TAppNews>(newsDto);
                news.Isdeleted = 0;
                news.Createddate = DateTime.UtcNow;
                // news.Createduser = GetCurrentUserId(); 

                var createdNews = await _newsRepository.AddAsync(news);

                return _mapper.Map<NewsDto>(createdNews);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Haber oluşturulurken bir hata oluştu.", ex);
            }
        }

        /// <inheritdoc />
        public async Task<NewsDto> UpdateNewsAsync(NewsDto newsDto)
        {
            if (newsDto?.Id == null || newsDto.Id <= 0)
                 throw new ArgumentNullException(nameof(newsDto), "Güncelleme için geçerli bir Haber ID'si gereklidir.");

            try
            {
                var existingNews = await _newsRepository.GetByIdAsync(newsDto.Id.Value);
                if (existingNews == null || existingNews.Isdeleted == 1)
                    throw new KeyNotFoundException($"Güncellenecek haber bulunamadı veya silinmiş: ID {newsDto.Id}");

                // Orijinal değerlerini koru
                var originalIsDeleted = existingNews.Isdeleted;
                var originalCreatedDate = existingNews.Createddate;
                var originalCreatedUser = existingNews.Createduser;

                _mapper.Map(newsDto, existingNews);

                // değerleri geri yükle
                existingNews.Isdeleted = originalIsDeleted;
                existingNews.Createddate = originalCreatedDate;
                existingNews.Createduser = originalCreatedUser;
                existingNews.Modifieddate = DateTime.UtcNow;
                // existingNews.Modifieduser = GetCurrentUserId(); 

                await _newsRepository.UpdateAsync(existingNews);
                
                return _mapper.Map<NewsDto>(existingNews);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Haber güncellenirken bir hata oluştu (ID: {newsDto.Id}).", ex);
            }
        }

        /// <inheritdoc />
        public async Task DeleteNewsAsync(int id)
        {
            try
            {
                await _newsRepository.SoftDeleteAsync(id);
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
    }
} 