using new_cms.Application.DTOs.NewsDTOs;
using new_cms.Application.DTOs.EventDTOs;
using new_cms.Application.DTOs.NoticeDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.Application.Interfaces
{    /// Haber, Etkinlik ve Duyuru içerik türlerinin yönetimini sağlayan arayüz
    public interface IContentService
    {
        /// Sayfalı ve filtrelenmiş haber listesi getirir.
        Task<(IEnumerable<NewsListDto> Items, int TotalCount)> GetPagedNewsAsync(
            int pageNumber, 
            int pageSize, 
            int? siteId = null, 
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true);
            
        /// Belirtilen ID'ye sahip haberi getirir.
        /// "id">Haber ID'si.
        Task<NewsDto?> GetNewsByIdAsync(int id);

        /// Yeni bir haber oluşturur.
        Task<NewsDto> CreateNewsAsync(NewsDto newsDto);

        /// Mevcut bir haberi günceller.
        Task<NewsDto> UpdateNewsAsync(NewsDto newsDto);

        /// Belirtilen ID'ye sahip haberi siler (soft delete).
        Task DeleteNewsAsync(int id);

        /// Sayfalı ve filtrelenmiş etkinlik listesi getirir.
        Task<(IEnumerable<EventListDto> Items, int TotalCount)> GetPagedEventsAsync(
            int pageNumber, 
            int pageSize, 
            int? siteId = null, 
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true);
            
        /// Belirtilen ID'ye sahip etkinliği getirir.
        Task<EventDto?> GetEventByIdAsync(int id);

        /// Yeni bir etkinlik oluşturur.
        Task<EventDto> CreateEventAsync(EventDto eventDto);

        /// Mevcut bir etkinliği günceller.
        Task<EventDto> UpdateEventAsync(EventDto eventDto);

        /// Belirtilen ID'ye sahip etkinliği siler (soft delete).
        Task DeleteEventAsync(int id);

        /// Sayfalı ve filtrelenmiş duyuru listesi getirir.
        Task<(IEnumerable<NoticeListDto> Items, int TotalCount)> GetPagedNoticesAsync(
            int pageNumber, 
            int pageSize, 
            int? siteId = null, 
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true);

        /// Belirtilen ID'ye sahip duyuruyu getirir.
        Task<NoticeDto?> GetNoticeByIdAsync(int id);

        /// Yeni bir duyuru oluşturur.
        Task<NoticeDto> CreateNoticeAsync(NoticeDto noticeDto);

        /// Mevcut bir duyuruyu günceller.
        Task<NoticeDto> UpdateNoticeAsync(NoticeDto noticeDto);
        
        /// Belirtilen ID'ye sahip duyuruyu siler (soft delete).
        Task DeleteNoticeAsync(int id);
    }
} 