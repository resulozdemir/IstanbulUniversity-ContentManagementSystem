using Microsoft.AspNetCore.Mvc;
using new_cms.Application.DTOs.NoticeDTOs;
using new_cms.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using new_cms.Application.DTOs.Common;

namespace new_cms.WebApi.Controllers
{
    /// Duyurular (Notices) ile ilgili API işlemlerini yönetir.
    [ApiController]
    [Route("api/[controller]")]
    public class NoticesController : ControllerBase
    {
        private readonly INoticeService _noticeService;
        public NoticesController(INoticeService noticeService)
        {
            _noticeService = noticeService;
        }


        /// Sayfalı, filtrelenmiş ve sıralanmış duyuru listesini getirir.
        /// <response code="200">Duyuru listesi başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz sayfalama veya filtre parametreleri.</response>
        /// <response code="500">Duyurular listelenirken sunucu hatası oluştu.</response>
        [HttpGet] // GET /api/notices?pageNumber=1&pageSize=15&siteId=1&searchTerm=önemli
        [ProducesResponseType(typeof(PaginatedResult<NoticeListDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PaginatedResult<NoticeListDto>>> GetPagedNotices(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? siteId = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortBy = "date", 
            [FromQuery] bool ascending = false) 
        {
             if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Sayfa numarası ve sayfa boyutu pozitif olmalıdır.");
            }

            try
            {
                var (items, totalCount) = await _noticeService.GetPagedNoticesAsync(pageNumber, pageSize, siteId, searchTerm, sortBy, ascending);
                var result = new PaginatedResult<NoticeListDto>(items, totalCount, pageNumber, pageSize);
                return Ok(result);
            }
            catch (InvalidOperationException ex) // Servis katmanından gelen genel hatalar
            {
                return StatusCode(500, $"Duyurular listelenirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Duyurular listelenirken beklenmedik bir sunucu hatası oluştu: {ex.Message}");
            }
        }


        /// Belirtilen ID'ye sahip aktif duyuruyu getirir.
        /// <response code="200">Duyuru başarıyla döndürüldü.</response>
        /// <response code="404">Belirtilen ID'ye sahip duyuru bulunamadı.</response>
        /// <response code="500">Duyuru getirilirken sunucu hatası oluştu.</response>
        [HttpGet("{id}")] // GET /api/notices/42
        [ProducesResponseType(typeof(NoticeDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<NoticeDto>> GetNoticeById(int id)
        {
             try
            {
                var notice = await _noticeService.GetNoticeByIdAsync(id);
                if (notice == null)
                {
                    return NotFound($"ID'si {id} olan duyuru bulunamadı.");
                }
                return Ok(notice);
            }
             catch (InvalidOperationException ex) // Servis katmanından gelen genel hatalar
            {
                 return StatusCode(500, $"Duyuru getirilirken bir hata oluştu (ID: {id}). Detay: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Duyuru getirilirken beklenmedik bir hata oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }


        /// Yeni bir duyuru oluşturur.
        /// <response code="201">Duyuru başarıyla oluşturuldu.</response>
        /// <response code="400">Geçersiz duyuru verisi gönderildi.</response>
        /// <response code="500">Duyuru oluşturulurken sunucu hatası oluştu.</response>
        [HttpPost] // POST /api/notices
        [ProducesResponseType(typeof(NoticeDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<NoticeDto>> CreateNotice([FromBody] NoticeDto noticeDto)
        {
             if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdNotice = await _noticeService.CreateNoticeAsync(noticeDto);
                
                return CreatedAtAction(nameof(GetNoticeById), new { id = createdNotice.Id }, createdNotice);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, $"Duyuru oluşturulurken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Duyuru oluşturulurken beklenmedik bir sunucu hatası oluştu: {ex.Message}");
            }
        }


        /// Mevcut bir duyuruyu günceller.
        /// <response code="200">Duyuru başarıyla güncellendi.</response>
        /// <response code="400">Gönderilen ID ile duyuru verisi uyuşmuyor veya geçersiz veri.</response>
        /// <response code="404">Güncellenecek duyuru bulunamadı.</response>
        /// <response code="500">Duyuru güncellenirken sunucu hatası oluştu.</response>
        [HttpPut("{id}")] // PUT /api/notices/42
        [ProducesResponseType(typeof(NoticeDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<NoticeDto>> UpdateNotice(int id, [FromBody] NoticeDto noticeDto)
        {
             if (noticeDto.Id == null) noticeDto.Id = id;
             else if (id != noticeDto.Id)
             {
                  return BadRequest("URL'deki ID ile gönderilen duyuru ID'si uyuşmuyor.");
             }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedNotice = await _noticeService.UpdateNoticeAsync(noticeDto);
                return Ok(updatedNotice);
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
                 return StatusCode(500, $"Duyuru güncellenirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Duyuru güncellenirken beklenmedik bir sunucu hatası oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }


        /// Belirtilen ID'ye sahip duyuruyu pasif hale getirir (soft delete).
        /// <response code="204">Duyuru başarıyla pasifleştirildi.</response>
        /// <response code="404">Pasifleştirilecek duyuru bulunamadı.</response>
        /// <response code="500">Duyuru silinirken sunucu hatası oluştu.</response>
        [HttpDelete("{id}")] // DELETE /api/notices/42
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteNotice(int id)
        {
            try
            {
                await _noticeService.DeleteNoticeAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                 return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                 // Servis katmanından gelen genel silme hataları
                return StatusCode(500, $"Duyuru silinirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Duyuru silinirken beklenmedik bir sunucu hatası oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }
    }
} 