using Microsoft.AspNetCore.Mvc;
using new_cms.Application.DTOs.SiteDTOs;
using new_cms.Application.DTOs.TemplateDTOs;
using new_cms.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.WebApi.Controllers
{
    /// Şablonlarla (Templates) ilgili API işlemlerini yönetir.
    [ApiController]
    [Route("api/[controller]")]
    public class TemplatesController : ControllerBase
    {
        private readonly ITemplateService _templateService;

        public TemplatesController(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        /// Tüm aktif şablonları listeler.
        /// <response code="200">Şablonların listesi başarıyla döndürüldü.</response>
        /// <response code="500">Şablonları listelerken sunucu hatası oluştu.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TemplateDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<TemplateDto>>> GetAllTemplates()
        {
            try
            {
                var templates = await _templateService.GetAllTemplatesAsync();
                return Ok(templates);
            }
            catch (Exception)
            {
                return StatusCode(500, "Şablonları alırken sunucu hatası oluştu.");
            }
        }

        /// Belirtilen ID'ye sahip aktif şablonu getirir.
        /// <response code="200">Şablon başarıyla döndürüldü.</response>
        /// <response code="404">Belirtilen ID'ye sahip şablon bulunamadı.</response>
        /// <response code="500">Şablon getirilirken sunucu hatası oluştu.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TemplateDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<TemplateDto>> GetTemplateById(int id)
        {
            try
            {
                var template = await _templateService.GetTemplateByIdAsync(id);
                if (template == null)
                {
                    return NotFound($"ID'si {id} olan şablon bulunamadı.");
                }
                return Ok(template);
            }
            catch (Exception)
            {
                return StatusCode(500, $"Şablon getirilirken bir hata oluştu (ID: {id}).");
            }
        }

        /// Yeni bir şablon oluşturur.
        /// <response code="201">Şablon başarıyla oluşturuldu.</response>
        /// <response code="400">Geçersiz şablon verisi gönderildi.</response>
        /// <response code="500">Şablon oluşturulurken sunucu hatası oluştu.</response>
        [HttpPost]
        [ProducesResponseType(typeof(TemplateDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<TemplateDto>> CreateTemplate([FromBody] TemplateDto templateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdTemplate = await _templateService.CreateTemplateAsync(templateDto);
                
                return CreatedAtAction(nameof(GetTemplateById), new { id = createdTemplate.Id }, createdTemplate);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Şablon oluşturulurken beklenmedik bir sunucu hatası oluştu.");
            }
        }

        /// Mevcut bir şablonu günceller.
        /// <response code="200">Şablon başarıyla güncellendi.</response>
        /// <response code="400">Gönderilen ID ile şablon verisi uyuşmuyor veya geçersiz veri.</response>
        /// <response code="404">Güncellenecek şablon bulunamadı.</response>
        /// <response code="500">Şablon güncellenirken sunucu hatası oluştu.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TemplateDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<TemplateDto>> UpdateTemplate(int id, [FromBody] TemplateDto templateDto)
        {
            if (id != templateDto.Id)
            {
                return BadRequest("URL'deki ID ile gönderilen şablon ID'si uyuşmuyor.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedTemplate = await _templateService.UpdateTemplateAsync(id, templateDto);
                return Ok(updatedTemplate);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, $"Şablon güncellenirken beklenmedik bir sunucu hatası oluştu (ID: {id}).");
            }
        }

        /// Belirtilen ID'ye sahip şablonu pasif hale getirir (soft delete).
        /// <response code="204">Şablon başarıyla pasifleştirildi.</response>
        /// <response code="404">Pasifleştirilecek şablon bulunamadı.</response>
        /// <response code="500">Şablon silinirken sunucu hatası oluştu.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            try
            {
                await _templateService.DeleteTemplateAsync(id);
                return NoContent(); 
            }
            catch (KeyNotFoundException ex) 
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, $"Şablon silinirken beklenmedik bir sunucu hatası oluştu (ID: {id}).");
            }
        }

        /// Belirli bir şablonu kullanan tüm aktif siteleri listeler.
        /// <response code="200">Şablonu kullanan sitelerin listesi başarıyla döndürüldü.</response>
        /// <response code="404">Belirtilen ID'ye sahip şablon bulunamadı.</response>
        /// <response code="500">Siteler listelenirken sunucu hatası oluştu.</response>
        [HttpGet("{id}/sites")]
        [ProducesResponseType(typeof(IEnumerable<SiteListDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SiteListDto>>> GetSitesByTemplate(int id)
        {
            try
            {
                var sites = await _templateService.GetSitesByTemplateAsync(id);
                return Ok(sites);
            }
            catch (Exception)
            {
                return StatusCode(500, $"Şablonu kullanan siteler listelenirken bir hata oluştu (Şablon ID: {id}).");
            }
        }

        /// Bir kaynak şablonun içeriğini (sayfalar, bileşenler vb.) hedef bir siteye kopyalar.
        /// <response code="204">Şablon içeriği siteye başarıyla kopyalandı.</response>
        /// <response code="400">Geçersiz kaynak şablonu veya hedef site ID'si.</response>
        /// <response code="404">Belirtilen şablon veya site bulunamadı.</response>
        /// <response code="500">Şablon içeriği kopyalanırken sunucu hatası oluştu.</response>
        [HttpPost("{sourceTemplateId}/copy-to-site/{targetSiteId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CopyTemplateContentToSite(int sourceTemplateId, int targetSiteId)
        {
            try
            {
                await _templateService.CopyTemplateContentToSiteAsync(sourceTemplateId, targetSiteId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, $"Şablon içeriği siteye kopyalanırken beklenmedik bir hata oluştu.");
            }
        }
    }
} 