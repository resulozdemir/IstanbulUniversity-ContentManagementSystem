using Microsoft.AspNetCore.Mvc;
using new_cms.Application.DTOs.NewsDTOs;
using new_cms.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using new_cms.Application.DTOs.Common;

namespace new_cms.WebApi.Controllers
{
    /// Haberler (News) ile ilgili API işlemlerini yönetir.
    [ApiController]
    [Route("api/[controller]")] 
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        public NewsController(INewsService newsService) 
        {
            _newsService = newsService;
        }

        /// Sayfalı, filtrelenmiş ve sıralanmış haber listesini getirir.
        /// <response code="200">Haber listesi başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz sayfalama veya filtre parametreleri.</response>
        /// <response code="500">Haberler listelenirken sunucu hatası oluştu.</response>
        [HttpGet] // GET /api/news?pageNumber=1&pageSize=20&siteId=1&sortBy=date&ascending=false
        [ProducesResponseType(typeof(PaginatedResult<NewsListDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PaginatedResult<NewsListDto>>> GetPagedNews(
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
                var (items, totalCount) = await _newsService.GetPagedNewsAsync(pageNumber, pageSize, siteId, searchTerm, sortBy, ascending);
                var result = new PaginatedResult<NewsListDto>(items, totalCount, pageNumber, pageSize);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                // Servis katmanından gelen genel hatalar
                return StatusCode(500, $"Haberler listelenirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Haberler listelenirken beklenmedik bir sunucu hatası oluştu: {ex.Message}");
            }
        }


        /// Belirtilen ID'ye sahip aktif haberi getirir.
        /// <response code="200">Haber başarıyla döndürüldü.</response>
        /// <response code="404">Belirtilen ID'ye sahip haber bulunamadı.</response>
        /// <response code="500">Haber getirilirken sunucu hatası oluştu.</response>
        [HttpGet("{id}")] // GET /api/news/15
        [ProducesResponseType(typeof(NewsDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<NewsDto>> GetNewsById(int id)
        {
            try
            {
                var news = await _newsService.GetNewsByIdAsync(id);
                if (news == null)
                {
                    return NotFound($"ID'si {id} olan haber bulunamadı.");
                }
                return Ok(news);
            }
            catch (InvalidOperationException ex)
            {
                // Servis katmanından gelen genel hatalar
                 return StatusCode(500, $"Haber getirilirken bir hata oluştu (ID: {id}). Detay: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Haber getirilirken beklenmedik bir hata oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }


        /// Yeni bir haber oluşturur.
        /// <response code="201">Haber başarıyla oluşturuldu.</response>
        /// <response code="400">Geçersiz haber verisi gönderildi.</response>
        /// <response code="500">Haber oluşturulurken sunucu hatası oluştu.</response>
        [HttpPost] // POST /api/news
        [ProducesResponseType(typeof(NewsDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<NewsDto>> CreateNews([FromBody] NewsDto newsDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdNews = await _newsService.CreateNewsAsync(newsDto);
                
                return CreatedAtAction(nameof(GetNewsById), new { id = createdNews.Id }, createdNews);
            }
            catch (InvalidOperationException ex)
            {
                 // Servis katmanından gelen genel hatalar
                return StatusCode(500, $"Haber oluşturulurken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Haber oluşturulurken beklenmedik bir sunucu hatası oluştu: {ex.Message}");
            }
        }


        /// Mevcut bir haberi günceller.
        /// <response code="200">Haber başarıyla güncellendi.</response>
        /// <response code="400">Gönderilen ID ile haber verisi uyuşmuyor veya geçersiz veri.</response>
        /// <response code="404">Güncellenecek haber bulunamadı.</response>
        /// <response code="500">Haber güncellenirken sunucu hatası oluştu.</response>
        [HttpPut("{id}")] // PUT /api/news/15
        [ProducesResponseType(typeof(NewsDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<NewsDto>> UpdateNews(int id, [FromBody] NewsDto newsDto)
        {
            // ID'lerin eşleştiğini kontrol et
            if (newsDto.Id == null) newsDto.Id = id;
            else if (id != newsDto.Id)
            {
                 return BadRequest("URL'deki ID ile gönderilen haber ID'si uyuşmuyor.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedNews = await _newsService.UpdateNewsAsync(newsDto);
                return Ok(updatedNews);
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
                 return StatusCode(500, $"Haber güncellenirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Haber güncellenirken beklenmedik bir sunucu hatası oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }


        /// Belirtilen ID'ye sahip haberi pasif hale getirir (soft delete).
        /// <response code="204">Haber başarıyla pasifleştirildi.</response>
        /// <response code="404">Pasifleştirilecek haber bulunamadı.</response>
        /// <response code="500">Haber silinirken sunucu hatası oluştu.</response>
        [HttpDelete("{id}")] // DELETE /api/news/15
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteNews(int id)
        {
            try
            {
                await _newsService.DeleteNewsAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex) 
            {
                return NotFound(ex.Message); 
            }
             catch (InvalidOperationException ex)
            {
                 // Servis katmanından gelen genel silme hataları
                return StatusCode(500, $"Haber silinirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Beklenmedik diğer hatalar (Loglama önerilir)
                return StatusCode(500, $"Haber silinirken beklenmedik bir sunucu hatası oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }
    }
} 