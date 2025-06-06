using new_cms.Application.DTOs.EventDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.Application.Interfaces
{
    /// Etkinlik (Event) içerik türünün yönetimini sağlayan arayüz.
    public interface IEventService
    {
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

        /// Belirtilen siteye ait etkinlikleri listeler.
        Task<IEnumerable<EventListDto>> GetEventsBySiteIdAsync(int siteId);

        /// Sistemdeki tüm aktif etkinlikleri listeler.
        Task<IEnumerable<EventListDto>> GetActiveEventsAsync();

        /// Yayınlanmış etkinlikleri listeler.
        Task<IEnumerable<EventListDto>> GetPublishedEventsAsync();
    }
} 