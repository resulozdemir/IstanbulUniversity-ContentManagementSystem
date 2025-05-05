using AutoMapper;
using Microsoft.EntityFrameworkCore;
using new_cms.Application.DTOs.EventDTOs;
using new_cms.Application.Interfaces;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace new_cms.Application.Services
{
    /// Etkinlik (Event) işlemleri için servis implementasyonu.
    public class EventService : IEventService
    {
        private readonly IRepository<TAppEvent> _eventRepository;
        private readonly IMapper _mapper;

        public EventService(
            IRepository<TAppEvent> eventRepository,
            IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<(IEnumerable<EventListDto> Items, int TotalCount)> GetPagedEventsAsync(
            int pageNumber, int pageSize, int? siteId = null, string? searchTerm = null, string? sortBy = null, bool ascending = true)
        {
             try
            {
                var query = _eventRepository.Query().Where(e => e.Isdeleted == 0);

                // Site ID'sine göre filtrele
                if (siteId.HasValue)
                {
                    query = query.Where(e => e.Siteid == siteId.Value);
                }

                // Arama terimine göre filtrele (Başlık, İçerik, Özet, Adres, Etiket)
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
                
                // Toplam öğe sayısını al
                var totalCount = await query.CountAsync();

                // Sayfalama uygula
                var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                return (_mapper.Map<IEnumerable<EventListDto>>(items), totalCount);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Etkinlikler listelenirken bir hata oluştu.", ex);
            }
        }

        /// <inheritdoc />
        public async Task<EventDto?> GetEventByIdAsync(int id)
        {
             try
            {
                // Belirtilen ID'ye sahip ve silinmemiş etkinliği bul
                var eventItem = await _eventRepository.Query().FirstOrDefaultAsync(e => e.Id == id && e.Isdeleted == 0);
                
                return eventItem == null ? null : _mapper.Map<EventDto>(eventItem);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Etkinlik getirilirken bir hata oluştu (ID: {id}).", ex);
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<EventDto> UpdateEventAsync(EventDto eventDto)
        {
            if (eventDto?.Id == null || eventDto.Id <= 0)
                 throw new ArgumentNullException(nameof(eventDto), "Güncelleme için geçerli bir Etkinlik ID'si gereklidir.");

            try
            {
                var existingEvent = await _eventRepository.GetByIdAsync(eventDto.Id.Value);
                
                 if (existingEvent == null || existingEvent.Isdeleted == 1)
                    throw new KeyNotFoundException($"Güncellenecek etkinlik bulunamadı veya silinmiş: ID {eventDto.Id}");

                // Orijinal değerlerini koru
                var originalIsDeleted = existingEvent.Isdeleted;
                var originalCreatedDate = existingEvent.Createddate;
                var originalCreatedUser = existingEvent.Createduser;

                _mapper.Map(eventDto, existingEvent);

                // orijinal değerleri geri yükle
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
                throw new InvalidOperationException($"Etkinlik güncellenirken bir hata oluştu (ID: {eventDto.Id}).", ex);
            }
        }

        /// <inheritdoc />
        public async Task DeleteEventAsync(int id)
        {
             try
            {
                await _eventRepository.SoftDeleteAsync(id);
            }
             catch (KeyNotFoundException ex) 
            {
                throw new KeyNotFoundException($"Silinecek etkinlik bulunamadı veya zaten silinmiş: ID {id}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Etkinlik silinirken bir hata oluştu (ID: {id}).", ex);
            }
        }
    }
} 