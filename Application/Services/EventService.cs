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
    /// Etkinlik (TAppEvent) varlıkları ile ilgili işlemleri gerçekleştiren servis sınıfı.
    public class EventService : IEventService
    {
                private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IIdGeneratorService _idGenerator;

        public EventService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IIdGeneratorService idGenerator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _idGenerator = idGenerator;
        }

        public async Task<(IEnumerable<EventListDto> Items, int TotalCount)> GetPagedEventsAsync(
            int pageNumber, int pageSize, int? siteId = null, string? searchTerm = null, string? sortBy = null, bool ascending = true)
        {
            // Sayfa ve boyut değerlerinin geçerliliğini kontrol et
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10; // Varsayılan boyut

            try
            {
                var query = _unitOfWork.Repository<TAppEvent>().Query().Where(e => e.Isdeleted == 0); 

                if (siteId.HasValue && siteId.Value > 0) 
                {
                    query = query.Where(e => e.Siteid == siteId.Value);
                }

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(e => 
                        (e.Header != null && e.Header.Contains(searchTerm)) || 
                        (e.Content != null && e.Content.Contains(searchTerm)) ||
                        (e.Summary != null && e.Summary.Contains(searchTerm)) ||
                        (e.Address != null && e.Address.Contains(searchTerm)) ||
                        (e.Tag != null && e.Tag.Contains(searchTerm))
                    );
                }
                
                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    query = sortBy.ToLowerInvariant() switch 
                    {
                        "id" => ascending ? query.OrderBy(e => e.Id) : query.OrderByDescending(e => e.Id),
                        "header" => ascending ? query.OrderBy(e => e.Header) : query.OrderByDescending(e => e.Header),
                        "date" => ascending ? query.OrderBy(e => e.Ondate) : query.OrderByDescending(e => e.Ondate),
                        "createddate" => ascending ? query.OrderBy(e => e.Createddate) : query.OrderByDescending(e => e.Createddate), 
                        "modifieddate" => ascending ? query.OrderBy(e => e.Modifieddate) : query.OrderByDescending(e => e.Modifieddate), 
                        "ispublish" => ascending ? query.OrderBy(e => e.Ispublish) : query.OrderByDescending(e => e.Ispublish), 
                        "priorityorder" => ascending ? query.OrderBy(e => e.Priorityorder) : query.OrderByDescending(e => e.Priorityorder), 
                        "address" => ascending ? query.OrderBy(e => e.Address) : query.OrderByDescending(e => e.Address),
                        _ => ascending ? query.OrderBy(n => n.Ondate) : query.OrderByDescending(n => n.Ondate) 
                    };
                }
                else
                {
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
            if (id <= 0) {
                 throw new ArgumentException("Geçerli bir etkinlik ID'si gereklidir.", nameof(id));
            }

             try
            {
                var eventItem = await _unitOfWork.Repository<TAppEvent>().Query()
                                    .FirstOrDefaultAsync(e => e.Id == id && e.Isdeleted == 0); 
                
                return eventItem == null ? null : _mapper.Map<EventDto>(eventItem);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Etkinlik getirilirken bir hata oluştu (ID: {id}).", ex);
            }
        }

        public async Task<EventDto> CreateEventAsync(EventDto eventDto)
        {
            if (eventDto == null) {
                 throw new ArgumentNullException(nameof(eventDto), "Oluşturulacak etkinlik bilgileri boş olamaz.");
            }
            if (string.IsNullOrWhiteSpace(eventDto.Header)) {
                 throw new ArgumentException("Etkinlik başlığı boş olamaz.", nameof(eventDto.Header));
            }
             if (eventDto.SiteId <= 0) { 
                 throw new ArgumentException("Geçerli bir Site ID'si gereklidir.", nameof(eventDto.SiteId));
             }

            try
            {
                var eventItem = _mapper.Map<TAppEvent>(eventDto);
                 
                eventItem.Id = await _idGenerator.GenerateNextIdAsync<TAppEvent>();
                eventItem.Isdeleted = 0; 
                eventItem.Createddate = DateTime.UtcNow; 
                // eventItem.Createduser = GetCurrentUserId(); // TODO: Aktif kullanıcı ID'si entegre edilmeli

                var createdEvent = await _unitOfWork.Repository<TAppEvent>().AddAsync(eventItem);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<EventDto>(createdEvent);
            }
            catch (DbUpdateException ex) {
                throw new InvalidOperationException($"Etkinlik oluşturulurken veritabanı hatası: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException || ex is ArgumentException) throw;
                throw new InvalidOperationException("Etkinlik oluşturulurken beklenmedik bir hata oluştu.", ex);
            }
        }

        public async Task<EventDto> UpdateEventAsync(EventDto eventDto)
        {
            if (eventDto?.Id == null || eventDto.Id <= 0)
                 throw new ArgumentNullException(nameof(eventDto), "Güncelleme için geçerli bir Etkinlik ID'si gereklidir.");
            if (string.IsNullOrWhiteSpace(eventDto.Header)) {
                 throw new ArgumentException("Etkinlik başlığı boş olamaz.", nameof(eventDto.Header));
            }
            if (eventDto.SiteId <= 0) { 
                 throw new ArgumentException("Geçerli bir Site ID'si gereklidir.", nameof(eventDto.SiteId));
            }

            try
            {
                var existingEvent = await _unitOfWork.Repository<TAppEvent>().GetByIdAsync(eventDto.Id.Value); 
                
                if (existingEvent == null || existingEvent.Isdeleted == 1)
                    throw new KeyNotFoundException($"Güncellenecek etkinlik bulunamadı veya silinmiş: ID {eventDto.Id.Value}");

                var originalIsDeleted = existingEvent.Isdeleted;
                var originalCreatedDate = existingEvent.Createddate;
                var originalCreatedUser = existingEvent.Createduser;

                _mapper.Map(eventDto, existingEvent);

                existingEvent.Isdeleted = originalIsDeleted;
                existingEvent.Createddate = originalCreatedDate;
                existingEvent.Createduser = originalCreatedUser;

                existingEvent.Modifieddate = DateTime.UtcNow;
                // existingEvent.Modifieduser = GetCurrentUserId(); // TODO: Aktif kullanıcı ID'si eklenmeli

                await _unitOfWork.Repository<TAppEvent>().UpdateAsync(existingEvent);
                await _unitOfWork.CompleteAsync();
                
                return _mapper.Map<EventDto>(existingEvent);
            }
            catch (DbUpdateException ex) {
                 throw new InvalidOperationException($"Etkinlik güncellenirken veritabanı hatası (ID: {eventDto.Id.Value}): {ex.InnerException?.Message ?? ex.Message}", ex);
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException || ex is ArgumentNullException || ex is ArgumentException) throw;
                throw new InvalidOperationException($"Etkinlik güncellenirken beklenmedik bir hata oluştu (ID: {eventDto.Id.Value}).", ex);
            }
        }

        public async Task DeleteEventAsync(int id)
        {
            if (id <= 0) {
                 throw new ArgumentException("Geçerli bir etkinlik ID'si gereklidir.", nameof(id));
            }

             try
            {
                // UnitOfWork üzerinden TAppEvent repository'sine erişim ve SoftDeleteAsync çağrısı
                // Bu metodun ilgili entity'yi bulup IsDeleted=1 ve ModifiedDate ayarladığını varsayıyoruz.
                await _unitOfWork.Repository<TAppEvent>().SoftDeleteAsync(id);

                await _unitOfWork.CompleteAsync();
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

        /// Belirtilen siteye ait etkinlikleri listeler.
        public async Task<IEnumerable<EventListDto>> GetEventsBySiteIdAsync(int siteId)
        {
            if (siteId <= 0)
            {
                throw new ArgumentException("Geçerli bir Site ID'si gereklidir.", nameof(siteId));
            }

            try
            {
                var siteEvents = await _unitOfWork.Repository<TAppEvent>().Query()
                    .Where(e => e.Siteid == siteId && e.Isdeleted == 0)
                    .OrderByDescending(e => e.Ondate)
                    .ToListAsync();
                    
                return _mapper.Map<IEnumerable<EventListDto>>(siteEvents);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Site etkinlikleri listelenirken hata oluştu (Site ID: {siteId}).", ex);
            }
        }

        /// Sistemdeki tüm aktif etkinlikleri listeler.
        public async Task<IEnumerable<EventListDto>> GetActiveEventsAsync()
        {
            try
            {
                var activeEvents = await _unitOfWork.Repository<TAppEvent>().Query()
                    .Where(e => e.Isdeleted == 0)
                    .OrderByDescending(e => e.Ondate)
                    .ToListAsync();
                    
                return _mapper.Map<IEnumerable<EventListDto>>(activeEvents);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Aktif etkinlikler listelenirken bir hata oluştu.", ex);
            }
        }

        /// Yayınlanmış etkinlikleri listeler.
        public async Task<IEnumerable<EventListDto>> GetPublishedEventsAsync()
        {
            try
            {
                var publishedEvents = await _unitOfWork.Repository<TAppEvent>().Query()
                    .Where(e => e.Isdeleted == 0 && e.Ispublish == 1)
                    .OrderByDescending(e => e.Ondate)
                    .ToListAsync();
                    
                return _mapper.Map<IEnumerable<EventListDto>>(publishedEvents);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Yayınlanan etkinlikler listelenirken bir hata oluştu.", ex);
            }
        }
    }
} 