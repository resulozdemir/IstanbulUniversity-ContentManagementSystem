 using Microsoft.AspNetCore.Mvc;
using new_cms.Application.DTOs.SitemapDTOs;
using new_cms.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using new_cms.Application.DTOs.Common;

namespace new_cms.WebApi.Controllers
{
    /// Site haritası (Sitemaps) ile ilgili API işlemlerini yönetir
    [ApiController]
    [Route("api/[controller]")]
    public class SitemapsController : ControllerBase
    {
        private readonly ISitemapService _sitemapService;

        public SitemapsController(ISitemapService sitemapService)
        {
            _sitemapService = sitemapService;
        }

        /// Sayfalı, filtrelenmiş ve sıralanmış site haritası listesini getirir
        /// <response code="200">Site haritası listesi başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz sayfalama veya filtre parametreleri.</response>
        /// <response code="500">Site haritası kayıtları listelenirken sunucu hatası oluştu.</response>
        [HttpGet] // GET /api/sitemaps?pageNumber=1&pageSize=10&siteId=1&domain=example.com&lang=tr
        [ProducesResponseType(typeof(PaginatedResult<SitemapListDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PaginatedResult<SitemapListDto>>> GetPagedSitemaps(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? siteId = null,
            [FromQuery] string? domain = null,
            [FromQuery] string? lang = null,
            [FromQuery] int? column1 = null, // İçerik tipi
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortBy = "id", // Varsayılan sıralama alanı
            [FromQuery] bool ascending = true) // Varsayılan artan sıralama
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Sayfa numarası ve sayfa boyutu pozitif olmalıdır.");
            }

            try
            {
                var (items, totalCount) = await _sitemapService.GetPagedSitemapsAsync(pageNumber, pageSize, siteId, domain, lang, column1, searchTerm, sortBy, ascending);
                var result = new PaginatedResult<SitemapListDto>(items, totalCount, pageNumber, pageSize);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                // Servis katmanından gelen genel hatalar
                return StatusCode(500, $"Site haritası kayıtları listelenirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Site haritası kayıtları listelenirken beklenmedik bir sunucu hatası oluştu: {ex.Message}");
            }
        }

        /// Sistemdeki tüm aktif site haritası kayıtlarını getirir
        /// <response code="200">Aktif site haritası kayıtları başarıyla döndürüldü.</response>
        /// <response code="500">Aktif site haritası kayıtları listelenirken sunucu hatası oluştu.</response>
        [HttpGet("active")] // GET /api/sitemaps/active
        [ProducesResponseType(typeof(IEnumerable<SitemapListDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SitemapListDto>>> GetActiveSitemaps()
        {
            try
            {
                var activeSitemaps = await _sitemapService.GetActiveSitemapsAsync();
                return Ok(activeSitemaps);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Aktif site haritası kayıtları listelenirken bir hata oluştu: {ex.Message}");
            }
        }

        /// Belirtilen site'ye ait site haritası kayıtlarını getirir
        /// <response code="200">Site'ye ait site haritası kayıtları başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz site ID'si.</response>
        /// <response code="500">Site'ye ait site haritası kayıtları listelenirken sunucu hatası oluştu.</response>
        [HttpGet("site/{siteId}")] // GET /api/sitemaps/site/1
        [ProducesResponseType(typeof(IEnumerable<SitemapListDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SitemapListDto>>> GetSitemapsBySiteId(int siteId)
        {
            if (siteId <= 0)
            {
                return BadRequest("Geçerli bir Site ID'si gereklidir.");
            }

            try
            {
                var sitemaps = await _sitemapService.GetSitemapsBySiteIdAsync(siteId);
                return Ok(sitemaps);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Site'ye ait site haritası kayıtları listelenirken bir hata oluştu (Site ID: {siteId}): {ex.Message}");
            }
        }

        /// Belirtilen domain'e ait site haritası kayıtlarını getirir
        /// <response code="200">Domain'e ait site haritası kayıtları başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz domain.</response>
        /// <response code="500">Domain'e ait site haritası kayıtları listelenirken sunucu hatası oluştu.</response>
        [HttpGet("domain/{domain}")] // GET /api/sitemaps/domain/example.com
        [ProducesResponseType(typeof(IEnumerable<SitemapListDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SitemapListDto>>> GetSitemapsByDomain(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                return BadRequest("Geçerli bir Domain gereklidir.");
            }

            try
            {
                var sitemaps = await _sitemapService.GetSitemapsByDomainAsync(domain);
                return Ok(sitemaps);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Domain'e ait site haritası kayıtları listelenirken bir hata oluştu (Domain: {domain}): {ex.Message}");
            }
        }

        /// Belirtilen dile ait site haritası kayıtlarını getirir
        /// <response code="200">Dile ait site haritası kayıtları başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz dil kodu.</response>
        /// <response code="500">Dile ait site haritası kayıtları listelenirken sunucu hatası oluştu.</response>
        [HttpGet("lang/{lang}")] // GET /api/sitemaps/lang/tr
        [ProducesResponseType(typeof(IEnumerable<SitemapListDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SitemapListDto>>> GetSitemapsByLang(string lang)
        {
            if (string.IsNullOrWhiteSpace(lang))
            {
                return BadRequest("Geçerli bir dil kodu gereklidir.");
            }

            try
            {
                var sitemaps = await _sitemapService.GetSitemapsByLangAsync(lang);
                return Ok(sitemaps);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Dile ait site haritası kayıtları listelenirken bir hata oluştu (Lang: {lang}): {ex.Message}");
            }
        }

        /// Belirtilen içerik tipi (Column1) için site haritası kayıtlarını getirir
        /// <response code="200">İçerik tipine ait site haritası kayıtları başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz içerik tipi.</response>
        /// <response code="500">İçerik tipine ait site haritası kayıtları listelenirken sunucu hatası oluştu.</response>
        [HttpGet("contenttype/{column1}")] // GET /api/sitemaps/contenttype/1
        [ProducesResponseType(typeof(IEnumerable<SitemapListDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SitemapListDto>>> GetSitemapsByContentType(int column1)
        {
            if (column1 <= 0)
            {
                return BadRequest("Geçerli bir içerik tipi gereklidir.");
            }

            try
            {
                var sitemaps = await _sitemapService.GetSitemapsByContentTypeAsync(column1);
                return Ok(sitemaps);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"İçerik tipine ait site haritası kayıtları listelenirken bir hata oluştu (Type: {column1}): {ex.Message}");
            }
        }

        /// Belirtilen URL için site haritası kaydını getirir
        /// <response code="200">URL'e ait site haritası kaydı başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz URL.</response>
        /// <response code="404">Belirtilen URL'e ait site haritası kaydı bulunamadı.</response>
        /// <response code="500">URL'e ait site haritası kaydı getirilirken sunucu hatası oluştu.</response>
        [HttpGet("url")] // GET /api/sitemaps/url?url=/haberler/teknoloji
        [ProducesResponseType(typeof(SitemapDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<SitemapDto>> GetSitemapByUrl([FromQuery] string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return BadRequest("Geçerli bir URL gereklidir.");
            }

            try
            {
                var sitemap = await _sitemapService.GetSitemapByUrlAsync(url);
                if (sitemap == null)
                {
                    return NotFound($"URL '{url}' için site haritası kaydı bulunamadı.");
                }
                return Ok(sitemap);
            }
            catch (InvalidOperationException ex)
            {
                // Servis katmanından gelen genel hatalar
                return StatusCode(500, $"URL'e ait site haritası kaydı getirilirken bir hata oluştu (URL: {url}). Detay: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"URL'e ait site haritası kaydı getirilirken beklenmedik bir hata oluştu (URL: {url}). Detay: {ex.Message}");
            }
        }

        /// Belirtilen ID'ye sahip site haritası kaydını getirir
        /// <response code="200">Site haritası kaydı başarıyla döndürüldü.</response>
        /// <response code="404">Belirtilen ID'ye sahip site haritası kaydı bulunamadı.</response>
        /// <response code="500">Site haritası kaydı getirilirken sunucu hatası oluştu.</response>
        [HttpGet("{id}")] // GET /api/sitemaps/15
        [ProducesResponseType(typeof(SitemapDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<SitemapDto>> GetSitemapById(int id)
        {
            try
            {
                var sitemap = await _sitemapService.GetSitemapByIdAsync(id);
                if (sitemap == null)
                {
                    return NotFound($"ID'si {id} olan site haritası kaydı bulunamadı.");
                }
                return Ok(sitemap);
            }
            catch (InvalidOperationException ex)
            {
                // Servis katmanından gelen genel hatalar
                return StatusCode(500, $"Site haritası kaydı getirilirken bir hata oluştu (ID: {id}). Detay: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Site haritası kaydı getirilirken beklenmedik bir hata oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }

        /// Yeni bir site haritası kaydı oluşturur
        /// <response code="201">Site haritası kaydı başarıyla oluşturuldu.</response>
        /// <response code="400">Geçersiz site haritası verisi gönderildi.</response>
        /// <response code="500">Site haritası kaydı oluşturulurken sunucu hatası oluştu.</response>
        [HttpPost] // POST /api/sitemaps
        [ProducesResponseType(typeof(SitemapDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<SitemapDto>> CreateSitemap([FromBody] SitemapDto sitemapDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdSitemap = await _sitemapService.CreateSitemapAsync(sitemapDto);
                
                return CreatedAtAction(nameof(GetSitemapById), new { id = createdSitemap.Id }, createdSitemap);
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
                // Servis katmanından gelen genel hatalar (duplicate URL vs.)
                return StatusCode(500, $"Site haritası kaydı oluşturulurken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Site haritası kaydı oluşturulurken beklenmedik bir sunucu hatası oluştu: {ex.Message}");
            }
        }

        /// Mevcut bir site haritası kaydını günceller
        /// <response code="200">Site haritası kaydı başarıyla güncellendi.</response>
        /// <response code="400">Gönderilen ID ile site haritası verisi uyuşmuyor veya geçersiz veri.</response>
        /// <response code="404">Güncellenecek site haritası kaydı bulunamadı.</response>
        /// <response code="500">Site haritası kaydı güncellenirken sunucu hatası oluştu.</response>
        [HttpPut("{id}")] // PUT /api/sitemaps/15
        [ProducesResponseType(typeof(SitemapDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<SitemapDto>> UpdateSitemap(int id, [FromBody] SitemapDto sitemapDto)
        {
            // ID'lerin eşleştiğini kontrol et
            if (sitemapDto.Id != id)
            {
                return BadRequest("URL'deki ID ile site haritası verisindeki ID eşleşmiyor.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedSitemap = await _sitemapService.UpdateSitemapAsync(sitemapDto);
                return Ok(updatedSitemap);
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
                // Servis katmanından gelen genel hatalar (duplicate URL vs.)
                return StatusCode(500, $"Site haritası kaydı güncellenirken bir hata oluştu (ID: {id}): {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Site haritası kaydı güncellenirken beklenmedik bir sunucu hatası oluştu (ID: {id}): {ex.Message}");
            }
        }

        /// Site haritası kaydını siler (soft delete)
        /// <response code="204">Site haritası kaydı başarıyla silindi.</response>
        /// <response code="404">Silinecek site haritası kaydı bulunamadı.</response>
        /// <response code="500">Site haritası kaydı silinirken sunucu hatası oluştu.</response>
        [HttpDelete("{id}")] // DELETE /api/sitemaps/15
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteSitemap(int id)
        {
            try
            {
                await _sitemapService.DeleteSitemapAsync(id);
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
                return StatusCode(500, $"Site haritası kaydı silinirken bir hata oluştu (ID: {id}): {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Site haritası kaydı silinirken beklenmedik bir sunucu hatası oluştu (ID: {id}): {ex.Message}");
            }
        }
    }
}