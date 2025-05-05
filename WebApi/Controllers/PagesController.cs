using Microsoft.AspNetCore.Mvc;
using new_cms.Application.DTOs.PageDTOs;
using new_cms.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.WebApi.Controllers
{
    /// Site sayfaları (Pages) ile ilgili API işlemlerini yönetir.
    [ApiController]
    [Route("api/[controller]")]
    public class PagesController : ControllerBase
    {
        private readonly IPageService _pageService;

        public PagesController(IPageService pageService)
        {
            _pageService = pageService ;
        }


        /// Belirtilen siteye ait tüm aktif sayfaları listeler.
        /// <response code="200">Sayfa listesi başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz site ID'si.</response>
        /// <response code="500">Sayfalar listelenirken sunucu hatası oluştu.</response>
        [HttpGet] // GET /api/pages?siteId=1
        [ProducesResponseType(typeof(IEnumerable<PageListDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<PageListDto>>> GetPagesBySiteId([FromQuery] int siteId)
        {
            if (siteId <= 0)
            {
                return BadRequest("Geçerli bir site ID'si gereklidir.");
            }

            try
            {
                var pages = await _pageService.GetPagesBySiteIdAsync(siteId);
                return Ok(pages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Siteye ait sayfalar listelenirken hata oluştu (Site ID: {siteId}). Detay: {ex.Message}");
            }
        }


        /// Belirtilen ID'ye sahip aktif sayfayı detaylarıyla getirir.
        /// <response code="200">Sayfa detayı başarıyla döndürüldü.</response>
        /// <response code="404">Belirtilen ID'ye sahip sayfa bulunamadı.</response>
        /// <response code="500">Sayfa detayı getirilirken sunucu hatası oluştu.</response>
        [HttpGet("{id}")] // GET /api/pages/101
        [ProducesResponseType(typeof(PageDetailDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PageDetailDto>> GetPageById(int id)
        {
             try
            {
                var page = await _pageService.GetPageByIdAsync(id);
                if (page == null)
                {
                    return NotFound($"ID'si {id} olan sayfa bulunamadı.");
                }
                return Ok(page);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sayfa detayı getirilirken hata oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }


        /// Yeni bir site sayfası oluşturur.
        /// <response code="201">Sayfa başarıyla oluşturuldu.</response>
        /// <response code="400">Geçersiz sayfa verisi gönderildi (örn: eksik SiteId, Name).</response>
        /// <response code="500">Sayfa oluşturulurken sunucu hatası oluştu.</response>
        [HttpPost] // POST /api/pages
        [ProducesResponseType(typeof(PageDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PageDto>> CreatePage([FromBody] PageDto pageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdPage = await _pageService.CreatePageAsync(pageDto);
                return CreatedAtAction(nameof(GetPageById), new { id = createdPage.Id }, createdPage);
            }
            catch (ArgumentException ex) // Geçersiz SiteId, Name vb.
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex) // DB veya diğer operasyonel hatalar
            {
                return StatusCode(500, $"Sayfa oluşturulurken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sayfa oluşturulurken beklenmedik bir sunucu hatası oluştu: {ex.Message}");
            }
        }


        /// Mevcut bir site sayfasının temel bilgilerini günceller.
        /// <response code="200">Sayfa başarıyla güncellendi.</response>
        /// <response code="400">Gönderilen ID ile sayfa verisi uyuşmuyor veya geçersiz veri.</response>
        /// <response code="404">Güncellenecek sayfa bulunamadı.</response>
        /// <response code="500">Sayfa güncellenirken sunucu hatası oluştu.</response>
        [HttpPut("{id}")] // PUT /api/pages/101
        [ProducesResponseType(typeof(PageDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PageDto>> UpdatePage(int id, [FromBody] PageDto pageDto)
        {
            if (id != pageDto.Id)
            {
                 if (pageDto.Id == null) pageDto.Id = id;
                 else if (id != pageDto.Id) return BadRequest("URL'deki ID ile gönderilen sayfa ID'si uyuşmuyor.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedPage = await _pageService.UpdatePageAsync(pageDto);
                return Ok(updatedPage);
            }
             catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                 return StatusCode(500, $"Sayfa güncellenirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sayfa güncellenirken beklenmedik bir sunucu hatası oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }


        /// Belirtilen ID'ye sahip sayfayı pasif hale getirir (soft delete).
        /// <response code="204">Sayfa başarıyla pasifleştirildi.</response>
        /// <response code="404">Pasifleştirilecek sayfa bulunamadı.</response>
        /// <response code="500">Sayfa silinirken sunucu hatası oluştu.</response>
        [HttpDelete("{id}")] // DELETE /api/pages/101
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeletePage(int id)
        {
            try
            {
                await _pageService.DeletePageAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex) when (ex.InnerException is KeyNotFoundException)
            {
                 return NotFound($"Silinecek sayfa bulunamadı (ID: {id}).");
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, $"Sayfa silinirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sayfa silinirken beklenmedik bir sunucu hatası oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }
    }
} 