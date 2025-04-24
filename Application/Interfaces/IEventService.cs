using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using new_cms.Application.DTOs.EventDTOs;

namespace new_cms.Application.Interfaces
{
    public interface IEventService
    {
        // Tüm etkinlikleri listeler
        Task<IEnumerable<EventListDto>> GetAllEventsAsync();
        
        // Belirli bir etkinlik detayını getirir
        Task<EventDetailDto?> GetEventByIdAsync(int id);
        
        // Yeni etkinlik oluşturur
        Task<EventDto> CreateEventAsync(EventDto eventDto);
        
        // Mevcut etkinliği günceller
        Task<EventDto> UpdateEventAsync(EventDto eventDto);
        
        // Etkinliği soft delete yapar
        Task DeleteEventAsync(int id);
        
        // Site ID'ye göre etkinlikleri listeler
        Task<IEnumerable<EventListDto>> GetEventsBySiteIdAsync(int siteId);
        
        // Kategori ID'ye göre etkinlikleri listeler
        Task<IEnumerable<EventListDto>> GetEventsByCategoryIdAsync(int categoryId);
        
        // URL'ye göre etkinlik getirir
        Task<EventDetailDto?> GetEventByUrlAsync(string url, int siteId);
        
        // Aktif (yayında) olan etkinlikleri listeler
        Task<IEnumerable<EventListDto>> GetActiveEventsAsync();
        
        // Sayfalı ve filtrelenmiş etkinlik listesi döndürür
        Task<(IEnumerable<EventListDto> Items, int TotalCount)> GetPagedEventsAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true);
            
        // Öne çıkan etkinlikleri listeler
        Task<IEnumerable<EventListDto>> GetFeaturedEventsAsync(int siteId);
        
        // Yaklaşan (gelecek) etkinlikleri listeler
        Task<IEnumerable<EventListDto>> GetUpcomingEventsAsync(int siteId, int count = 5);
        
        // Geçmiş etkinlikleri listeler
        Task<IEnumerable<EventListDto>> GetPastEventsAsync(int siteId);
        
        // Belirli bir tarih aralığındaki etkinlikleri listeler
        Task<IEnumerable<EventListDto>> GetEventsByDateRangeAsync(int siteId, DateTime startDate, DateTime endDate);
        
        // Konum (şehir/ilçe) bilgisine göre etkinlikleri listeler
        Task<IEnumerable<EventListDto>> GetEventsByLocationAsync(string location, int siteId);
    }
} 