using Microsoft.AspNetCore.Mvc;
using new_cms.Application.DTOs.SiteDTOs;
using new_cms.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.WebApi.Controllers
{
    /// Site alan adları (domain) ile ilgili API işlemlerini yönetir.
    [ApiController]
    [Route("api/[controller]")]
    public class SiteDomainsController : ControllerBase
    {
        private readonly ISiteDomainService _siteDomainService;

        public SiteDomainsController(ISiteDomainService siteDomainService)
        {
            _siteDomainService = siteDomainService;
        }


        /// Belirtilen siteye ait tüm aktif alan adlarını listeler.
        /// <response code="200">Alan adı listesi başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz site ID'si.</response>
        /// <response code="500">Alan adlarını listelerken sunucu hatası oluştu.</response>
        [HttpGet("bysite/{siteId}")] // GET /api/sitedomains/bysite/1
        [ProducesResponseType(typeof(IEnumerable<SiteDomainDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SiteDomainDto>>> GetDomainsBySiteId(int siteId)
        {
            if (siteId <= 0)
            {
                return BadRequest("Geçerli bir site ID'si gereklidir.");
            }

            try
            {
                var domains = await _siteDomainService.GetDomainsBySiteIdAsync(siteId);
                return Ok(domains);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Siteye ait alan adları listelenirken hata oluştu (Site ID: {siteId}). Detay: {ex.Message}");
            }
        }


        /// Belirtilen dile ait tüm aktif alan adlarını listeler.
        /// <response code="200">Alan adı listesi başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz dil kodu.</response>
        /// <response code="500">Alan adlarını listelerken sunucu hatası oluştu.</response>
        [HttpGet("bylanguage/{language}")] // GET /api/sitedomains/bylanguage/tr
        [ProducesResponseType(typeof(IEnumerable<SiteDomainDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SiteDomainDto>>> GetDomainsByLanguage(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
            {
                return BadRequest("Geçerli bir dil kodu gereklidir.");
            }

            try
            {
                var domains = await _siteDomainService.GetDomainsByLanguageAsync(language);
                return Ok(domains);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Dile göre alan adları listelenirken hata oluştu (Dil: {language}). Detay: {ex.Message}");
            }
        }


        /// Alan adına göre aktif kaydı getirir.
        /// <response code="200">Alan adı başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz alan adı.</response>
        /// <response code="404">Belirtilen alan adı bulunamadı.</response>
        /// <response code="500">Alan adı getirilirken sunucu hatası oluştu.</response>
        [HttpGet("lookup")] // GET /api/sitedomains/lookup?domain=example.com
        [ProducesResponseType(typeof(SiteDomainDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<SiteDomainDto>> GetByDomain([FromQuery] string domain)
        {
             if (string.IsNullOrWhiteSpace(domain))
            {
                return BadRequest("Geçerli bir alan adı gereklidir.");
            }
            try
            {
                var domainDto = await _siteDomainService.GetByDomainAsync(domain);
                if (domainDto == null)
                {
                    return NotFound($"'{domain}' alan adı bulunamadı.");
                }
                return Ok(domainDto);
            }
             catch (Exception ex)
            {
                return StatusCode(500, $"Alan adı getirilirken hata oluştu (Domain: {domain}). Detay: {ex.Message}");
            }
        }



        /// Yeni bir site alan adı oluşturur.
        /// <response code="201">Alan adı başarıyla oluşturuldu.</response>
        /// <response code="400">Geçersiz alan adı verisi gönderildi.</response>
        /// <response code="409">Alan adı zaten kullanımda (Conflict).</response>
        /// <response code="500">Alan adı oluşturulurken sunucu hatası oluştu.</response>
        [HttpPost] // POST /api/sitedomains
        [ProducesResponseType(typeof(SiteDomainDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<SiteDomainDto>> CreateDomain([FromBody] SiteDomainDto domainDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdDomain = await _siteDomainService.CreateDomainAsync(domainDto);
                
                return StatusCode(201, createdDomain);
            }
            catch (ArgumentException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex) // Domain zaten kullanılıyor veya DB hatası olabilir
            {
                 if(ex.Message.Contains("zaten kullanılıyor")) 
                     return Conflict(ex.Message);
                 else
                    return StatusCode(500, $"Alan adı oluşturulurken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Alan adı oluşturulurken beklenmedik bir sunucu hatası oluştu: {ex.Message}");
            }
        }


        /// Mevcut bir site alan adını günceller. Domain adının kendisi güncellenemez.
        /// <response code="200">Alan adı başarıyla güncellendi.</response>
        /// <response code="400">Gönderilen ID ile alan adı verisi uyuşmuyor veya geçersiz veri.</response>
        /// <response code="404">Güncellenecek alan adı bulunamadı.</response>
        /// <response code="500">Alan adı güncellenirken sunucu hatası oluştu.</response>
        [HttpPut("{id}")] // PUT /api/sitedomains/5
        [ProducesResponseType(typeof(SiteDomainDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<SiteDomainDto>> UpdateDomain(int id, [FromBody] SiteDomainDto domainDto)
        {
            if (id != domainDto.Id)
            {
                return BadRequest("URL'deki ID ile gönderilen alan adı ID'si uyuşmuyor.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedDomain = await _siteDomainService.UpdateDomainAsync(domainDto);
                return Ok(updatedDomain);
            }
            catch (ArgumentException ex) // Geçersiz ID veya diğer argüman hataları
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex) // DB veya başka operasyonel hata
            {
                 return StatusCode(500, $"Alan adı güncellenirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Alan adı güncellenirken beklenmedik bir sunucu hatası oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }


        /// Belirtilen ID'ye sahip alan adını pasif hale getirir (soft delete).
        /// <response code="204">Alan adı başarıyla pasifleştirildi.</response>
        /// <response code="404">Pasifleştirilecek alan adı bulunamadı.</response>
        /// <response code="500">Alan adı silinirken sunucu hatası oluştu.</response>
        [HttpDelete("{id}")] // DELETE /api/sitedomains/5
        [ProducesResponseType(204)]
        [ProducesResponseType(404)] 
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteDomain(int id)
        {
            try
            {
                await _siteDomainService.DeleteDomainAsync(id);
                return NoContent();
            }
             catch (InvalidOperationException ex) when (ex.InnerException is KeyNotFoundException) // Eğer servis KeyNotFound fırlatırsa
             {
                  return NotFound($"Silinecek alan adı bulunamadı (ID: {id}).");
             }
            catch (InvalidOperationException ex) // Servisten beklenen genel hata
            {
                return StatusCode(500, $"Alan adı silinirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Alan adı silinirken beklenmedik bir sunucu hatası oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }


        /// Bir alan adının benzersiz olup olmadığını kontrol eder.
        /// <response code="200">Kontrol sonucu başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz alan adı.</response>
        /// <response code="500">Kontrol sırasında sunucu hatası oluştu.</response>
        [HttpGet("checkunique")] // GET /api/sitedomains/checkunique?domain=test.com&excludeDomainId=5
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<bool>> IsDomainUnique([FromQuery] string domain, [FromQuery] int? excludeDomainId = null)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                return BadRequest("Kontrol edilecek alan adı boş olamaz.");
            }
            try
            {
                bool isUnique = await _siteDomainService.IsDomainUniqueAsync(domain, excludeDomainId);
                return Ok(isUnique);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Alan adı benzersizlik kontrolü sırasında hata oluştu: {ex.Message}");
            }
        }
    }
} 