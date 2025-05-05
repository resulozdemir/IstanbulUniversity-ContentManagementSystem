using Microsoft.AspNetCore.Mvc;
using new_cms.Application.DTOs.EventDTOs;
using new_cms.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using new_cms.Application.DTOs.Common;

namespace new_cms.WebApi.Controllers
{
    /// Etkinlikler (Events) ile ilgili API işlemlerini yönetir.
    [ApiController]
    [Route("api/[controller]")]  
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService; 
        public EventsController(IEventService eventService) 
        {
            _eventService = eventService;
        }


        /// Sayfalı, filtrelenmiş ve sıralanmış etkinlik listesini getirir.
        /// <response code="200">Etkinlik listesi başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz sayfalama veya filtre parametreleri.</response>
        /// <response code="500">Etkinlikler listelenirken sunucu hatası oluştu.</response>
        [HttpGet] // GET /api/events?pageNumber=1&pageSize=10&siteId=2&sortBy=priority&ascending=true
        [ProducesResponseType(typeof(PaginatedResult<EventListDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PaginatedResult<EventListDto>>> GetPagedEvents(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? siteId = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortBy = "date", // Varsayılan sıralama alanı
            [FromQuery] bool ascending = false) // Varsayılan azalan sıralama
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Sayfa numarası ve sayfa boyutu pozitif olmalıdır.");
            }

            try
            {
                var (items, totalCount) = await _eventService.GetPagedEventsAsync(pageNumber, pageSize, siteId, searchTerm, sortBy, ascending);
                var result = new PaginatedResult<EventListDto>(items, totalCount, pageNumber, pageSize);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                // Servis katmanından gelen genel hatalar
                return StatusCode(500, $"Etkinlikler listelenirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Etkinlikler listelenirken beklenmedik bir sunucu hatası oluştu: {ex.Message}");
            }
        }


        /// Belirtilen ID'ye sahip aktif etkinliği getirir.
        /// <response code="200">Etkinlik başarıyla döndürüldü.</response>
        /// <response code="404">Belirtilen ID'ye sahip etkinlik bulunamadı.</response>
        /// <response code="500">Etkinlik getirilirken sunucu hatası oluştu.</response>
        [HttpGet("{id}")] // GET /api/events/3
        [ProducesResponseType(typeof(EventDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<EventDto>> GetEventById(int id)
        {
            try
            {
                var eventItem = await _eventService.GetEventByIdAsync(id);
                if (eventItem == null)
                {
                    return NotFound($"ID'si {id} olan etkinlik bulunamadı.");
                }
                return Ok(eventItem);
            }
             catch (InvalidOperationException ex)
            {
                return StatusCode(500, $"Etkinlik getirilirken bir hata oluştu (ID: {id}). Detay: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Etkinlik getirilirken beklenmedik bir hata oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }


        /// Yeni bir etkinlik oluşturur.
        /// <response code="201">Etkinlik başarıyla oluşturuldu.</response>
        /// <response code="400">Geçersiz etkinlik verisi gönderildi.</response>
        /// <response code="500">Etkinlik oluşturulurken sunucu hatası oluştu.</response>
        [HttpPost] // POST /api/events
        [ProducesResponseType(typeof(EventDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<EventDto>> CreateEvent([FromBody] EventDto eventDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdEvent = await _eventService.CreateEventAsync(eventDto);
                
                return CreatedAtAction(nameof(GetEventById), new { id = createdEvent.Id }, createdEvent);
            }
            catch (InvalidOperationException ex)
            {
                 // Servis katmanından gelen genel hatalar
                return StatusCode(500, $"Etkinlik oluşturulurken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Etkinlik oluşturulurken beklenmedik bir sunucu hatası oluştu: {ex.Message}");
            }
        }


        /// Mevcut bir etkinliği günceller.
        /// <response code="200">Etkinlik başarıyla güncellendi.</response>
        /// <response code="400">Gönderilen ID ile etkinlik verisi uyuşmuyor veya geçersiz veri.</response>
        /// <response code="404">Güncellenecek etkinlik bulunamadı.</response>
        /// <response code="500">Etkinlik güncellenirken sunucu hatası oluştu.</response>
        [HttpPut("{id}")] // PUT /api/events/3
        [ProducesResponseType(typeof(EventDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<EventDto>> UpdateEvent(int id, [FromBody] EventDto eventDto)
        {
             if (eventDto.Id == null) eventDto.Id = id;
             else if (id != eventDto.Id)
             {
                  return BadRequest("URL'deki ID ile gönderilen etkinlik ID'si uyuşmuyor.");
             }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedEvent = await _eventService.UpdateEventAsync(eventDto);
                return Ok(updatedEvent);
            }
             catch (ArgumentNullException ex)
            {
                 // Servisin fırlattığı null argüman hatası (ID kontrolü için)
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                 // Servisin fırlattığı bulunamadı hatası
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                  // Servis katmanından gelen genel güncelleme hataları
                 return StatusCode(500, $"Etkinlik güncellenirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Etkinlik güncellenirken beklenmedik bir sunucu hatası oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }


        /// Belirtilen ID'ye sahip etkinliği pasif hale getirir (soft delete).
        /// <response code="204">Etkinlik başarıyla pasifleştirildi.</response>
        /// <response code="404">Pasifleştirilecek etkinlik bulunamadı.</response>
        /// <response code="500">Etkinlik silinirken sunucu hatası oluştu.</response>
        [HttpDelete("{id}")] // DELETE /api/events/3
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                await _eventService.DeleteEventAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex) 
            {
                 return NotFound(ex.Message); 
            }
            catch (InvalidOperationException ex)
            {
                // Servis katmanından gelen genel silme hataları
                return StatusCode(500, $"Etkinlik silinirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Etkinlik silinirken beklenmedik bir sunucu hatası oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }
    }
} 