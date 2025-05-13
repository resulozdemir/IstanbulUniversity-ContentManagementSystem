using Microsoft.AspNetCore.Mvc;
using new_cms.Application.DTOs.SiteDTOs;
using new_cms.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using new_cms.Application.DTOs.Common;

namespace new_cms.WebApi.Controllers
{
    /// Siteler (Sites) ile ilgili API işlemlerini yönetir.
    [ApiController]
    [Route("api/[controller]")]
    public class SitesController : ControllerBase
    {
        private readonly ISiteService _siteService;

        public SitesController(ISiteService siteService)
        {
            _siteService = siteService;
        }


        /// Tüm aktif siteleri listeler (Domain bilgileriyle birlikte).
        /// <response code="200">Site listesi başarıyla döndürüldü.</response>
        /// <response code="500">Siteler listelenirken sunucu hatası oluştu.</response>
        [HttpGet] // GET /api/sites
        [ProducesResponseType(typeof(IEnumerable<SiteListDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SiteListDto>>> GetAllSites()
        {
            try
            {
                var sites = await _siteService.GetAllSitesAsync();
                return Ok(sites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Tüm siteler listelenirken bir hata oluştu: {ex.Message}");
            }
        }


        /// Sayfalı olarak aktif siteleri listeler.
        /// <response code="200">Site listesi başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz sayfalama parametreleri.</response>
        /// <response code="500">Siteler listelenirken sunucu hatası oluştu.</response>
        [HttpGet("paged")] // GET /api/sites/paged?pageNumber=1&pageSize=5&sortBy=name
        [ProducesResponseType(typeof(PaginatedResult<SiteListDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PaginatedResult<SiteListDto>>> GetPagedSites(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? sortBy = "createddate", 
            [FromQuery] bool ascending = false)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest("Sayfa numarası ve sayfa boyutu pozitif olmalıdır.");
            }

            try
            {
                var (items, totalCount) = await _siteService.GetPagedSitesAsync(
                    pageNumber, pageSize, searchTerm, sortBy, ascending);
                var result = new PaginatedResult<SiteListDto>(items, totalCount, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sayfalı siteler listelenirken bir hata oluştu: {ex.Message}");
            }
        }


        /// Belirtilen ID'ye sahip aktif siteyi detaylarıyla getirir.
        /// <response code="200">Site detayı başarıyla döndürüldü.</response>
        /// <response code="404">Belirtilen ID'ye sahip site bulunamadı.</response>
        /// <response code="500">Site detayı getirilirken sunucu hatası oluştu.</response>
        [HttpGet("{id}")] // GET /api/sites/5
        [ProducesResponseType(typeof(SiteDetailDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<SiteDetailDto>> GetSiteById(int id)
        {
            try
            {
                var site = await _siteService.GetSiteByIdAsync(id);
                if (site == null)
                {
                    return NotFound($"ID'si {id} olan site bulunamadı.");
                }
                return Ok(site);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Site detayı getirilirken hata oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }
        

        /// Belirtilen alan adına (domain) sahip aktif siteyi detaylarıyla getirir.
        /// <response code="200">Site detayı başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz alan adı.</response>
        /// <response code="404">Belirtilen alan adına sahip site bulunamadı.</response>
        /// <response code="500">Site getirilirken sunucu hatası oluştu.</response>
        [HttpGet("bydomain")] // GET /api/sites/bydomain?domain=example.com
        [ProducesResponseType(typeof(SiteDetailDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<SiteDetailDto>> GetSiteByDomain([FromQuery] string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                return BadRequest("Geçerli bir alan adı gereklidir.");
            }
            try
            {
                var site = await _siteService.GetSiteByDomainAsync(domain);
                if (site == null)
                {
                    return NotFound($"'{domain}' alan adına sahip site bulunamadı.");
                }
                return Ok(site);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Alan adına göre site getirilirken hata oluştu (Domain: {domain}). Detay: {ex.Message}");
            }
        }


        /// Tüm aktif ve yayında olan (published) siteleri listeler.
        /// <response code="200">Yayındaki site listesi başarıyla döndürüldü.</response>
        /// <response code="500">Yayındaki siteler listelenirken sunucu hatası oluştu.</response>
        [HttpGet("published")] // GET /api/sites/published
        [ProducesResponseType(typeof(IEnumerable<SiteListDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SiteListDto>>> GetPublishedSites()
        {
             try
            {
                var sites = await _siteService.GetPublishedSitesAsync();
                return Ok(sites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Yayındaki siteler listelenirken bir hata oluştu: {ex.Message}");
            }
        }


        /// Tüm aktif site şablonlarını (template) listeler.
        /// <response code="200">Şablon listesi başarıyla döndürüldü.</response>
        /// <response code="500">Şablonlar listelenirken sunucu hatası oluştu.</response>
        [HttpGet("templates")] // GET /api/sites/templates
        [ProducesResponseType(typeof(IEnumerable<SiteListDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SiteListDto>>> GetSiteTemplates()
        {
             try
            {
                var templates = await _siteService.GetSiteTemplatesAsync();
                return Ok(templates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Site şablonları listelenirken bir hata oluştu: {ex.Message}");
            }
        }


        /// Belirtilen şablonu kullanan aktif siteleri listeler.
        /// <response code="200">Site listesi başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz şablon ID'si.</response>
        /// <response code="500">Siteler listelenirken sunucu hatası oluştu.</response>
        [HttpGet("bytemplate/{templateId}")] // GET /api/sites/bytemplate/2
        [ProducesResponseType(typeof(IEnumerable<SiteListDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SiteListDto>>> GetSitesByTemplate(int templateId)
        {
            if (templateId <= 0)
            {
                return BadRequest("Geçerli bir şablon ID'si gereklidir.");
            }
            try
            {
                var sites = await _siteService.GetSitesByTemplateAsync(templateId);
                return Ok(sites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Şablona göre siteler listelenirken hata oluştu (Template ID: {templateId}). Detay: {ex.Message}");
            }
        }


        /// Yeni bir site oluşturur (ve varsayılan alan adını kaydeder).
        /// <response code="201">Site başarıyla oluşturuldu.</response>
        /// <response code="400">Geçersiz site verisi gönderildi (örn: eksik Domain).</response>
        /// <response code="409">Belirtilen Domain zaten kullanımda (Conflict).</response>
        /// <response code="500">Site oluşturulurken sunucu hatası oluştu.</response>
        [HttpPost] // POST /api/sites
        [ProducesResponseType(typeof(SiteDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<SiteDto>> CreateSite([FromBody] SiteDto siteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdSite = await _siteService.CreateSiteAsync(siteDto);
                return CreatedAtAction(nameof(GetSiteById), new { id = createdSite.Id }, createdSite);
            }
            catch (ArgumentException ex) // Eksik domain gibi hatalar
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex) // Domain zaten var veya DB hatası
            {
                 if (ex.Message.Contains("zaten kullanılıyor"))
                     return Conflict(ex.Message);
                 else
                    return StatusCode(500, $"Site oluşturulurken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Site oluşturulurken beklenmedik bir sunucu hatası oluştu: {ex.Message}");
            }
        }


        /// Mevcut bir sitenin bilgilerini günceller.
        /// <response code="200">Site başarıyla güncellendi.</response>
        /// <response code="400">Gönderilen ID ile site verisi uyuşmuyor veya geçersiz veri.</response>
        /// <response code="404">Güncellenecek site bulunamadı.</response>
        /// <response code="500">Site güncellenirken sunucu hatası oluştu.</response>
        [HttpPut("{id}")] // PUT /api/sites/5
        [ProducesResponseType(typeof(SiteDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<SiteDto>> UpdateSite(int id, [FromBody] SiteDto siteDto)
        {
             if (id != siteDto.Id)
            {
                 if (siteDto.Id == null) siteDto.Id = id;
                 else if (id != siteDto.Id) return BadRequest("URL'deki ID ile gönderilen site ID'si uyuşmuyor.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedSite = await _siteService.UpdateSiteAsync(siteDto);
                return Ok(updatedSite);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
             catch (InvalidOperationException ex) // Beklenmedik DB veya diğer operasyonel hatalar
            {
                 return StatusCode(500, $"Site güncellenirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Site güncellenirken beklenmedik bir sunucu hatası oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }


        /// Belirtilen ID'ye sahip siteyi pasif hale getirir (soft delete).
        /// <response code="204">Site başarıyla pasifleştirildi.</response>
        /// <response code="404">Pasifleştirilecek site bulunamadı.</response>
        /// <response code="500">Site silinirken sunucu hatası oluştu.</response>
        [HttpDelete("{id}")] // DELETE /api/sites/5
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteSite(int id)
        {
            try
            {
                await _siteService.DeleteSiteAsync(id);
                return NoContent();
            }
             catch (InvalidOperationException ex) when (ex.InnerException is KeyNotFoundException)
            {
                 return NotFound($"Silinecek site bulunamadı (ID: {id}).");
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, $"Site silinirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Site silinirken beklenmedik bir sunucu hatası oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }


        /// Belirtilen ID'ye sahip siteyi yayına alır.
        /// <response code="200">Site başarıyla yayına alındı.</response>
        /// <response code="404">Yayına alınacak site bulunamadı.</response>
        /// <response code="500">Site yayına alınırken sunucu hatası oluştu.</response>
        [HttpPost("{id}/publish")] // POST /api/sites/5/publish
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PublishSite(int id)
        {
            try
            {
                await _siteService.PublishSiteAsync(id);
                return Ok($"ID'si {id} olan site başarıyla yayına alındı.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, $"Site yayınlanırken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Site yayınlanırken beklenmedik bir sunucu hatası oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }


        /// Belirtilen ID'ye sahip siteyi yayından kaldırır.
        /// <response code="200">Site başarıyla yayından kaldırıldı.</response>
        /// <response code="404">Yayından kaldırılacak site bulunamadı.</response>
        /// <response code="500">Site yayından kaldırılırken sunucu hatası oluştu.</response>
        [HttpPost("{id}/unpublish")] // POST /api/sites/5/unpublish
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UnpublishSite(int id)
        {
            try
            {
                await _siteService.UnpublishSiteAsync(id);
                 return Ok($"ID'si {id} olan site başarıyla yayından kaldırıldı.");
            }
             catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, $"Site yayından kaldırılırken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Site yayından kaldırılırken beklenmedik bir sunucu hatası oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }
    }
} 