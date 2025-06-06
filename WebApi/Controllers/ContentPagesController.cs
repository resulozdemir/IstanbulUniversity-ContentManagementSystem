using Microsoft.AspNetCore.Mvc;
using new_cms.Application.DTOs.ContentPageDTOs;
using new_cms.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using new_cms.Application.DTOs.Common;

namespace new_cms.WebApi.Controllers
{
    /// Content sayfaları (ContentPages) ile ilgili API işlemlerini yönetir
    [ApiController]
    [Route("api/[controller]")]
    public class ContentPagesController : ControllerBase
    {
        private readonly IContentPageService _contentPageService;

        public ContentPagesController(IContentPageService contentPageService)
        {
            _contentPageService = contentPageService;
        }

        /// Sayfalı, filtrelenmiş ve sıralanmış content sayfası listesini getirir
        /// <response code="200">Content sayfası listesi başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz sayfalama veya filtre parametreleri.</response>
        /// <response code="500">Content sayfaları listelenirken sunucu hatası oluştu.</response>
        [HttpGet] // GET /api/contentpages?pageNumber=1&pageSize=10&groupId=1&sortBy=orderby
        [ProducesResponseType(typeof(PaginatedResult<ContentPageListDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PaginatedResult<ContentPageListDto>>> GetPagedContentPages(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? groupId = null,
            [FromQuery] int? siteId = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortBy = "orderby", // Varsayılan sıralama alanı ORDERBY
            [FromQuery] bool ascending = true) // Varsayılan artan sıralama
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Sayfa numarası ve sayfa boyutu pozitif olmalıdır.");
            }

            try
            {
                var (items, totalCount) = await _contentPageService.GetPagedContentPagesAsync(pageNumber, pageSize, groupId, siteId, searchTerm, sortBy, ascending);
                var result = new PaginatedResult<ContentPageListDto>(items, totalCount, pageNumber, pageSize);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                // Servis katmanından gelen genel hatalar
                return StatusCode(500, $"Content sayfaları listelenirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Content sayfaları listelenirken beklenmedik bir sunucu hatası oluştu: {ex.Message}");
            }
        }

        /// Sistemdeki tüm aktif content sayfalarını getirir
        /// <response code="200">Aktif content sayfaları başarıyla döndürüldü.</response>
        /// <response code="500">Aktif content sayfaları listelenirken sunucu hatası oluştu.</response>
        [HttpGet("active")] // GET /api/contentpages/active
        [ProducesResponseType(typeof(IEnumerable<ContentPageListDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ContentPageListDto>>> GetActiveContentPages()
        {
            try
            {
                var activeContentPages = await _contentPageService.GetActiveContentPagesAsync();
                return Ok(activeContentPages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Aktif content sayfaları listelenirken bir hata oluştu: {ex.Message}");
            }
        }

        /// Belirtilen group'a ait content sayfalarını OrderBy alanına göre sıralı olarak getirir
        /// <response code="200">Group content sayfaları başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz group ID'si.</response>
        /// <response code="500">Group content sayfaları listelenirken sunucu hatası oluştu.</response>
        [HttpGet("group/{groupId}")] // GET /api/contentpages/group/1
        [ProducesResponseType(typeof(IEnumerable<ContentPageListDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ContentPageListDto>>> GetContentPagesByGroupId(int groupId)
        {
            if (groupId <= 0)
            {
                return BadRequest("Geçerli bir Group ID'si gereklidir.");
            }

            try
            {
                var contentPages = await _contentPageService.GetContentPagesByGroupIdAsync(groupId);
                return Ok(contentPages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Group content sayfaları listelenirken bir hata oluştu (Group ID: {groupId}): {ex.Message}");
            }
        }

        /// Belirtilen siteye ait content sayfalarını getirir
        /// <response code="200">Site content sayfaları başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz site ID'si.</response>
        /// <response code="500">Site content sayfaları listelenirken sunucu hatası oluştu.</response>
        [HttpGet("site/{siteId}")] // GET /api/contentpages/site/1
        [ProducesResponseType(typeof(IEnumerable<ContentPageListDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ContentPageListDto>>> GetContentPagesBySiteId(int siteId)
        {
            if (siteId <= 0)
            {
                return BadRequest("Geçerli bir Site ID'si gereklidir.");
            }

            try
            {
                var contentPages = await _contentPageService.GetContentPagesBySiteIdAsync(siteId);
                return Ok(contentPages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Site content sayfaları listelenirken bir hata oluştu (Site ID: {siteId}): {ex.Message}");
            }
        }

        /// Belirtilen ID'ye sahip content sayfasını getirir
        /// <response code="200">Content sayfası başarıyla döndürüldü.</response>
        /// <response code="404">Belirtilen ID'ye sahip content sayfası bulunamadı.</response>
        /// <response code="500">Content sayfası getirilirken sunucu hatası oluştu.</response>
        [HttpGet("{id}")] // GET /api/contentpages/15
        [ProducesResponseType(typeof(ContentPageDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ContentPageDto>> GetContentPageById(int id)
        {
            try
            {
                var contentPage = await _contentPageService.GetContentPageByIdAsync(id);
                if (contentPage == null)
                {
                    return NotFound($"ID'si {id} olan content sayfası bulunamadı.");
                }
                return Ok(contentPage);
            }
            catch (InvalidOperationException ex)
            {
                // Servis katmanından gelen genel hatalar
                return StatusCode(500, $"Content sayfası getirilirken bir hata oluştu (ID: {id}). Detay: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Content sayfası getirilirken beklenmedik bir hata oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }

        /// Yeni bir content sayfası oluşturur
        /// <response code="201">Content sayfası başarıyla oluşturuldu.</response>
        /// <response code="400">Geçersiz content sayfası verisi gönderildi.</response>
        /// <response code="500">Content sayfası oluşturulurken sunucu hatası oluştu.</response>
        [HttpPost] // POST /api/contentpages
        [ProducesResponseType(typeof(ContentPageDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ContentPageDto>> CreateContentPage([FromBody] ContentPageDto contentPageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdContentPage = await _contentPageService.CreateContentPageAsync(contentPageDto);
                
                return CreatedAtAction(nameof(GetContentPageById), new { id = createdContentPage.Id }, createdContentPage);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // Servis katmanından gelen genel hatalar
                return StatusCode(500, $"Content sayfası oluşturulurken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Content sayfası oluşturulurken beklenmedik bir sunucu hatası oluştu: {ex.Message}");
            }
        }

        /// Mevcut bir content sayfasını günceller
        /// <response code="200">Content sayfası başarıyla güncellendi.</response>
        /// <response code="400">Gönderilen ID ile content sayfası verisi uyuşmuyor veya geçersiz veri.</response>
        /// <response code="404">Güncellenecek content sayfası bulunamadı.</response>
        /// <response code="500">Content sayfası güncellenirken sunucu hatası oluştu.</response>
        [HttpPut("{id}")] // PUT /api/contentpages/15
        [ProducesResponseType(typeof(ContentPageDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ContentPageDto>> UpdateContentPage(int id, [FromBody] ContentPageDto contentPageDto)
        {
            // ID'lerin eşleştiğini kontrol et
            if (contentPageDto.Id != id)
            {
                return BadRequest("URL'deki ID ile content sayfası verisindeki ID eşleşmiyor.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedContentPage = await _contentPageService.UpdateContentPageAsync(contentPageDto);
                return Ok(updatedContentPage);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // Servis katmanından gelen genel hatalar
                return StatusCode(500, $"Content sayfası güncellenirken bir hata oluştu (ID: {id}): {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Content sayfası güncellenirken beklenmedik bir sunucu hatası oluştu (ID: {id}): {ex.Message}");
            }
        }

        /// Content sayfasını siler (soft delete)
        /// <response code="204">Content sayfası başarıyla silindi.</response>
        /// <response code="404">Silinecek content sayfası bulunamadı.</response>
        /// <response code="500">Content sayfası silinirken sunucu hatası oluştu.</response>
        [HttpDelete("{id}")] // DELETE /api/contentpages/15
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteContentPage(int id)
        {
            try
            {
                await _contentPageService.DeleteContentPageAsync(id);
                return NoContent(); // 204 No Content response
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // Servis katmanından gelen genel hatalar
                return StatusCode(500, $"Content sayfası silinirken bir hata oluştu (ID: {id}): {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Content sayfası silinirken beklenmedik bir sunucu hatası oluştu (ID: {id}): {ex.Message}");
            }
        }
    }
} 