using AutoMapper;
using Microsoft.EntityFrameworkCore;
using new_cms.Application.DTOs.EventDTOs;
using new_cms.Application.DTOs.NewsDTOs;
using new_cms.Application.DTOs.NoticeDTOs;
using new_cms.Application.Interfaces;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace new_cms.Application.Services
{
    /// Haber, Etkinlik ve Duyuru içerik türlerinin yönetimini sağlayan servis.
    public class ContentService : IContentService
    {
        private readonly IRepository<TAppNews> _newsRepository;
        private readonly IRepository<TAppEvent> _eventRepository;
        private readonly IRepository<TAppNotice> _noticeRepository;
        private readonly IMapper _mapper;

        public ContentService(
            IRepository<TAppNews> newsRepository,
            IRepository<TAppEvent> eventRepository,
            IRepository<TAppNotice> noticeRepository,
            IMapper mapper)
        {
            _newsRepository = newsRepository;
            _eventRepository = eventRepository;
            _noticeRepository = noticeRepository;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<NewsListDto> Items, int TotalCount)> GetPagedNewsAsync(
            int pageNumber, int pageSize, int? siteId = null, string? searchTerm = null, string? sortBy = null, bool ascending = true)
        {
            try
            {
                var query = _newsRepository.Query().Where(n => n.Isdeleted == 0);

                if (siteId.HasValue)
                {
                    query = query.Where(n => n.Siteid == siteId.Value);
                }

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

                // Sayfalama
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

        public async Task<NewsDto> UpdateNewsAsync(NewsDto newsDto)
        {
            if (newsDto?.Id == null || newsDto.Id <= 0)
                 throw new ArgumentNullException(nameof(newsDto), "Güncelleme için geçerli bir Haber ID'si gereklidir.");

            try
            {
                var existingNews = await _newsRepository.GetByIdAsync(newsDto.Id.Value);
                if (existingNews == null || existingNews.Isdeleted == 1)
                    throw new KeyNotFoundException($"Güncellenecek haber bulunamadı veya silinmiş: ID {newsDto.Id}");

                var originalIsDeleted = existingNews.Isdeleted;
                var originalCreatedDate = existingNews.Createddate;
                var originalCreatedUser = existingNews.Createduser;

                _mapper.Map(newsDto, existingNews);

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
                 if (ex is KeyNotFoundException) throw;
                throw new InvalidOperationException($"Haber güncellenirken bir hata oluştu (ID: {newsDto.Id}).", ex);
            }
        }

        public async Task DeleteNewsAsync(int id)
        {
            try
            {
                await _newsRepository.SoftDeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Haber silinirken bir hata oluştu (ID: {id}).", ex);
            }
        }

        // --- Event Operations --- 

        public async Task<(IEnumerable<EventListDto> Items, int TotalCount)> GetPagedEventsAsync(
            int pageNumber, int pageSize, int? siteId = null, string? searchTerm = null, string? sortBy = null, bool ascending = true)
        {
             try
            {
                var query = _eventRepository.Query().Where(e => e.Isdeleted == 0);

                if (siteId.HasValue)
                {
                    query = query.Where(e => e.Siteid == siteId.Value);
                }

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(e => e.Header.Contains(searchTerm) || 
                                             (e.Content != null && e.Content.Contains(searchTerm)) ||
                                             (e.Summary != null && e.Summary.Contains(searchTerm)) ||
                                             (e.Address != null && e.Address.Contains(searchTerm)) ||
                                             (e.Tag != null && e.Tag.Contains(searchTerm)));
                }
                
                // Dinamik sıralama
                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    query = sortBy.ToLower() switch
                    {
                        "id" => ascending ? query.OrderBy(e => e.Id) : query.OrderByDescending(e => e.Id),
                        "header" => ascending ? query.OrderBy(e => e.Header) : query.OrderByDescending(e => e.Header),
                        "date" => ascending ? query.OrderBy(e => e.Ondate) : query.OrderByDescending(e => e.Ondate),
                        "created" => ascending ? query.OrderBy(e => e.Createddate) : query.OrderByDescending(e => e.Createddate),
                        "modified" => ascending ? query.OrderBy(e => e.Modifieddate) : query.OrderByDescending(e => e.Modifieddate),
                        "publish" => ascending ? query.OrderBy(e => e.Ispublish) : query.OrderByDescending(e => e.Ispublish),
                        "priority" => ascending ? query.OrderBy(e => e.Priorityorder) : query.OrderByDescending(e => e.Priorityorder),
                        "address" => ascending ? query.OrderBy(e => e.Address) : query.OrderByDescending(e => e.Address),
                        _ => ascending ? query.OrderBy(e => e.Ondate) : query.OrderByDescending(e => e.Ondate) // Varsayılan tarih sıralaması
                    };
                }
                else
                {
                    // Varsayılan sıralama (sortBy belirtilmemişse)
                    query = ascending ? query.OrderBy(e => e.Ondate) : query.OrderByDescending(e => e.Ondate);
                }
                
                var totalCount = await query.CountAsync();
                var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                return (_mapper.Map<IEnumerable<EventListDto>>(items), totalCount);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Etkinlikler listelenirken bir hata oluştu.", ex);
            }
        }

        public async Task<EventDto?> GetEventByIdAsync(int id)
        {
             try
            {
                var eventItem = await _eventRepository.Query().FirstOrDefaultAsync(e => e.Id == id && e.Isdeleted == 0);
                return eventItem == null ? null : _mapper.Map<EventDto>(eventItem);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Etkinlik getirilirken bir hata oluştu (ID: {id}).", ex);
            }
        }

        public async Task<EventDto> CreateEventAsync(EventDto eventDto)
        {
            try
            {
                var eventItem = _mapper.Map<TAppEvent>(eventDto);
                eventItem.Isdeleted = 0;
                eventItem.Createddate = DateTime.UtcNow;
                // eventItem.Createduser = GetCurrentUserId();

                var createdEvent = await _eventRepository.AddAsync(eventItem);
                return _mapper.Map<EventDto>(createdEvent);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Etkinlik oluşturulurken bir hata oluştu.", ex);
            }
        }

        public async Task<EventDto> UpdateEventAsync(EventDto eventDto)
        {
            if (eventDto?.Id == null || eventDto.Id <= 0)
                 throw new ArgumentNullException(nameof(eventDto), "Güncelleme için geçerli bir Etkinlik ID'si gereklidir.");

            try
            {
                var existingEvent = await _eventRepository.GetByIdAsync(eventDto.Id.Value);
                 if (existingEvent == null || existingEvent.Isdeleted == 1)
                    throw new KeyNotFoundException($"Güncellenecek etkinlik bulunamadı veya silinmiş: ID {eventDto.Id}");

                var originalIsDeleted = existingEvent.Isdeleted;
                var originalCreatedDate = existingEvent.Createddate;
                var originalCreatedUser = existingEvent.Createduser;

                _mapper.Map(eventDto, existingEvent);

                existingEvent.Isdeleted = originalIsDeleted;
                existingEvent.Createddate = originalCreatedDate;
                existingEvent.Createduser = originalCreatedUser;
                existingEvent.Modifieddate = DateTime.UtcNow;
                // existingEvent.Modifieduser = GetCurrentUserId();

                await _eventRepository.UpdateAsync(existingEvent);
                return _mapper.Map<EventDto>(existingEvent);
            }
            catch (Exception ex)
            {
                 if (ex is KeyNotFoundException) throw;
                throw new InvalidOperationException($"Etkinlik güncellenirken bir hata oluştu (ID: {eventDto.Id}).", ex);
            }
        }

        public async Task DeleteEventAsync(int id)
        {
             try
            {
                await _eventRepository.SoftDeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Etkinlik silinirken bir hata oluştu (ID: {id}).", ex);
            }
        }

        // --- Notice Operations --- 

        public async Task<(IEnumerable<NoticeListDto> Items, int TotalCount)> GetPagedNoticesAsync(
            int pageNumber, int pageSize, int? siteId = null, string? searchTerm = null, string? sortBy = null, bool ascending = true)
        {
            try
            {
                var query = _noticeRepository.Query().Where(n => n.Isdeleted == 0);

                if (siteId.HasValue)
                {
                    query = query.Where(n => n.Siteid == siteId.Value);
                }

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
                        "category" => ascending ? query.OrderBy(n => n.Categoryid) : query.OrderByDescending(n => n.Categoryid),
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

                return (_mapper.Map<IEnumerable<NoticeListDto>>(items), totalCount);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Duyurular listelenirken bir hata oluştu.", ex);
            }
        }

        public async Task<NoticeDto?> GetNoticeByIdAsync(int id)
        {
            try
            {
                var notice = await _noticeRepository.Query().FirstOrDefaultAsync(n => n.Id == id && n.Isdeleted == 0);
                return notice == null ? null : _mapper.Map<NoticeDto>(notice);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Duyuru getirilirken bir hata oluştu (ID: {id}).", ex);
            }
        }

        public async Task<NoticeDto> CreateNoticeAsync(NoticeDto noticeDto)
        {
             try
            {
                var notice = _mapper.Map<TAppNotice>(noticeDto);
                notice.Isdeleted = 0;
                notice.Createddate = DateTime.UtcNow;
                // notice.Createduser = GetCurrentUserId(); // TODO

                var createdNotice = await _noticeRepository.AddAsync(notice);
                return _mapper.Map<NoticeDto>(createdNotice);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Duyuru oluşturulurken bir hata oluştu.", ex);
            }
        }

        public async Task<NoticeDto> UpdateNoticeAsync(NoticeDto noticeDto)
        {
            if (noticeDto?.Id == null || noticeDto.Id <= 0)
                 throw new ArgumentNullException(nameof(noticeDto), "Güncelleme için geçerli bir Duyuru ID'si gereklidir.");

             try
            {
                var existingNotice = await _noticeRepository.GetByIdAsync(noticeDto.Id);
                if (existingNotice == null || existingNotice.Isdeleted == 1)
                    throw new KeyNotFoundException($"Güncellenecek duyuru bulunamadı veya silinmiş: ID {noticeDto.Id}");

                var originalIsDeleted = existingNotice.Isdeleted;
                var originalCreatedDate = existingNotice.Createddate;
                var originalCreatedUser = existingNotice.Createduser;

                _mapper.Map(noticeDto, existingNotice);

                existingNotice.Isdeleted = originalIsDeleted;
                existingNotice.Createddate = originalCreatedDate;
                existingNotice.Createduser = originalCreatedUser;
                existingNotice.Modifieddate = DateTime.UtcNow;
                // existingNotice.Modifieduser = GetCurrentUserId();

                await _noticeRepository.UpdateAsync(existingNotice);
                return _mapper.Map<NoticeDto>(existingNotice);
            }
            catch (Exception ex)
            {
                 if (ex is KeyNotFoundException) throw;
                throw new InvalidOperationException($"Duyuru güncellenirken bir hata oluştu (ID: {noticeDto.Id}).", ex);
            }
        }

        public async Task DeleteNoticeAsync(int id)
        {
            try
            {
                await _noticeRepository.SoftDeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Duyuru silinirken bir hata oluştu (ID: {id}).", ex);
            }
        }
    }
} 