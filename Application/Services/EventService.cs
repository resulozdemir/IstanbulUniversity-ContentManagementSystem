using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using new_cms.Application.DTOs.EventDTOs;
using new_cms.Application.Interfaces;
using new_cms.Domain.Entities;

namespace new_cms.Application.Services
{
    public class EventService : IEventService
    {
        private readonly UCmsContext _context;
        private readonly IMapper _mapper;

        public EventService(UCmsContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Tüm etkinlikleri listeler
        public async Task<IEnumerable<EventListDto>> GetAllEventsAsync()
        {
            var events = await _context.TAppEvents
                .Where(e => e.Isdeleted == 0)
                .OrderByDescending(e => e.Ondate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EventListDto>>(events);
        }

        // Belirli bir etkinlik detayını getirir
        public async Task<EventDetailDto?> GetEventByIdAsync(int id)
        {
            var eventEntity = await _context.TAppEvents
                .Include(e => e.Site)
                .FirstOrDefaultAsync(e => e.Id == id && e.Isdeleted == 0);

            if (eventEntity == null)
                return null;

            var eventDetail = _mapper.Map<EventDetailDto>(eventEntity);
            
            // Etiketleri ayırıp listeye ekle
            if (!string.IsNullOrEmpty(eventEntity.Tag))
            {
                // Virgülle ayrılmış etiketleri string listesine dönüştür
                eventDetail.Tags = eventEntity.Tag
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .ToList();
            }

            return eventDetail;
        }

        // Yeni etkinlik oluşturur
        public async Task<EventDto> CreateEventAsync(EventDto eventDto)
        {
            var eventEntity = _mapper.Map<TAppEvent>(eventDto);
            
            // Oluşturma bilgilerini ayarla
            eventEntity.Createddate = DateTime.Now;
            eventEntity.Createduser = 1; // Sistemin varsayılan kullanıcısı veya mevcut kullanıcı ID'si eklenebilir
            eventEntity.Isdeleted = 0;
            
            _context.TAppEvents.Add(eventEntity);
            await _context.SaveChangesAsync();

            return _mapper.Map<EventDto>(eventEntity);
        }

        // Mevcut etkinliği günceller
        public async Task<EventDto> UpdateEventAsync(EventDto eventDto)
        {
            var eventEntity = await _context.TAppEvents.FindAsync(eventDto.Id);
            if (eventEntity == null)
                throw new KeyNotFoundException($"Etkinlik bulunamadı: ID {eventDto.Id}");

            _mapper.Map(eventDto, eventEntity);
            
            // Güncelleme bilgilerini ayarla
            eventEntity.Modifieddate = DateTime.Now;
            eventEntity.Modifieduser = 1; // Sistemin varsayılan kullanıcısı veya mevcut kullanıcı ID'si eklenebilir

            _context.TAppEvents.Update(eventEntity);
            await _context.SaveChangesAsync();

            return _mapper.Map<EventDto>(eventEntity);
        }

        // Etkinliği soft delete yapar
        public async Task DeleteEventAsync(int id)
        {
            var eventEntity = await _context.TAppEvents.FindAsync(id);
            if (eventEntity == null)
                throw new KeyNotFoundException($"Etkinlik bulunamadı: ID {id}");

            eventEntity.Isdeleted = 1;
            eventEntity.Modifieddate = DateTime.Now;
            eventEntity.Modifieduser = 1; // Sistemin varsayılan kullanıcısı veya mevcut kullanıcı ID'si eklenebilir

            await _context.SaveChangesAsync();
        }

        // Site ID'ye göre etkinlikleri listeler
        public async Task<IEnumerable<EventListDto>> GetEventsBySiteIdAsync(int siteId)
        {
            var events = await _context.TAppEvents
                .Where(e => e.Siteid == siteId && e.Isdeleted == 0)
                .OrderByDescending(e => e.Ondate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EventListDto>>(events);
        }

        // Kategori ID'ye göre etkinlikleri listeler
        public async Task<IEnumerable<EventListDto>> GetEventsByCategoryIdAsync(int categoryId)
        {
            // Bu örnekte kategoriye göre filtreleme uygulanıyor.
            // Gerçek uygulamada kategori tablosu ile ilişkili bir alan üzerinden filtreleme yapılmalıdır.
            var events = await _context.TAppEvents
                .Where(e => e.Isdeleted == 0)
                .OrderByDescending(e => e.Ondate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EventListDto>>(events);
        }

        // URL'ye göre etkinlik getirir
        public async Task<EventDetailDto?> GetEventByUrlAsync(string url, int siteId)
        {
            // Bu örnekte Link alanına göre filtreleme yapılıyor.
            // Gerçek uygulamada SEO URL tablosu veya alanı üzerinden filtreleme yapılabilir.
            var eventEntity = await _context.TAppEvents
                .Include(e => e.Site)
                .FirstOrDefaultAsync(e => e.Link.Contains(url) && e.Siteid == siteId && e.Isdeleted == 0);

            if (eventEntity == null)
                return null;

            var eventDetail = _mapper.Map<EventDetailDto>(eventEntity);
            
            // Etiketleri ayırıp listeye ekle
            if (!string.IsNullOrEmpty(eventEntity.Tag))
            {
                // Virgülle ayrılmış etiketleri string listesine dönüştür
                eventDetail.Tags = eventEntity.Tag
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .ToList();
            }

            return eventDetail;
        }

        // Aktif olan etkinlikleri listeler
        public async Task<IEnumerable<EventListDto>> GetActiveEventsAsync()
        {
            var events = await _context.TAppEvents
                .Where(e => e.Isdeleted == 0 && e.Ispublish == 1)
                .OrderByDescending(e => e.Ondate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EventListDto>>(events);
        }

        // Sayfalı ve filtrelenmiş etkinlik listesi döndürür
        public async Task<(IEnumerable<EventListDto> Items, int TotalCount)> GetPagedEventsAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true)
        {
            // Temel sorgu
            var query = _context.TAppEvents.Where(e => e.Isdeleted == 0);

            // Arama terimine göre filtreleme
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(e => e.Header.Contains(searchTerm) || 
                                    e.Content.Contains(searchTerm) ||
                                    e.Tag.Contains(searchTerm) ||
                                    e.Address.Contains(searchTerm));
            }

            // Toplam kayıt sayısını al
            var totalCount = await query.CountAsync();

            // Sıralama
            IQueryable<TAppEvent> sortedQuery;
            if (string.IsNullOrEmpty(sortBy))
            {
                sortedQuery = ascending 
                    ? query.OrderBy(e => e.Ondate)
                    : query.OrderByDescending(e => e.Ondate);
            }
            else
            {
                // Sıralama alanına göre sıralama
                sortedQuery = sortBy.ToLower() switch
                {
                    "header" => ascending 
                        ? query.OrderBy(e => e.Header)
                        : query.OrderByDescending(e => e.Header),
                    "date" => ascending 
                        ? query.OrderBy(e => e.Ondate)
                        : query.OrderByDescending(e => e.Ondate),
                    _ => ascending 
                        ? query.OrderBy(e => e.Ondate)
                        : query.OrderByDescending(e => e.Ondate)
                };
            }

            // Sayfalama
            var pagedItems = await sortedQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<EventListDto>>(pagedItems);
            return (dtos, totalCount);
        }

        // Öne çıkan etkinlikleri listeler
        public async Task<IEnumerable<EventListDto>> GetFeaturedEventsAsync(int siteId)
        {
            // Öne çıkan etkinlikleri getir (Öncelik sırasına göre)
            var events = await _context.TAppEvents
                .Where(e => e.Siteid == siteId && e.Isdeleted == 0 && e.Ispublish == 1)
                .OrderByDescending(e => e.Priorityorder)
                .ThenByDescending(e => e.Ondate)
                .Take(5)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EventListDto>>(events);
        }

        // Yaklaşan (gelecek) etkinlikleri listeler
        public async Task<IEnumerable<EventListDto>> GetUpcomingEventsAsync(int siteId, int count = 5)
        {
            var currentDate = DateTime.Now.Date;
            
            var events = await _context.TAppEvents
                .Where(e => e.Siteid == siteId && 
                       e.Isdeleted == 0 && 
                       e.Ispublish == 1 &&
                       e.Ondate >= currentDate)
                .OrderBy(e => e.Ondate)
                .Take(count)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EventListDto>>(events);
        }

        // Geçmiş etkinlikleri listeler
        public async Task<IEnumerable<EventListDto>> GetPastEventsAsync(int siteId)
        {
            var currentDate = DateTime.Now.Date;
            
            var events = await _context.TAppEvents
                .Where(e => e.Siteid == siteId && 
                       e.Isdeleted == 0 && 
                       e.Ispublish == 1 &&
                       e.Ondate < currentDate)
                .OrderByDescending(e => e.Ondate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EventListDto>>(events);
        }

        // Belirli bir tarih aralığındaki etkinlikleri listeler
        public async Task<IEnumerable<EventListDto>> GetEventsByDateRangeAsync(int siteId, DateTime startDate, DateTime endDate)
        {
            var events = await _context.TAppEvents
                .Where(e => e.Siteid == siteId && 
                       e.Isdeleted == 0 && 
                       e.Ondate >= startDate &&
                       e.Ondate <= endDate)
                .OrderByDescending(e => e.Ondate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EventListDto>>(events);
        }

        // Konum (şehir/ilçe) bilgisine göre etkinlikleri listeler
        public async Task<IEnumerable<EventListDto>> GetEventsByLocationAsync(string location, int siteId)
        {
            var events = await _context.TAppEvents
                .Where(e => e.Siteid == siteId && 
                       e.Isdeleted == 0 && 
                       e.Address.Contains(location))
                .OrderByDescending(e => e.Ondate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EventListDto>>(events);
        }
    }
} 