using Microsoft.AspNetCore.Mvc;
using new_cms.Application.DTOs.ComponentDTOs;
using new_cms.Application.DTOs.ThemeDTOs;
using new_cms.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.WebApi.Controllers
{
    /// Bileşenler ve bileşen verileri ile ilgili API işlemlerini yönetir.
    [ApiController]
    [Route("api/[controller]")]
    public class ComponentsController : ControllerBase
    {
        private readonly IComponentService _componentService;
        
        public ComponentsController(IComponentService componentService)
        {
            _componentService = componentService;
        }


        /// Bir bileşeni belirli bir temaya ekler (ilişkilendirir).
        /// <response code="201">İlişki başarıyla oluşturuldu.</response>
        /// <response code="400">Geçersiz veri gönderildi (örn: eksik ID'ler).</response>
        /// <response code="409">Bu bileşen zaten bu temaya atanmış (Conflict).</response>
        /// <response code="500">İlişki oluşturulurken sunucu hatası oluştu.</response>
        [HttpPost("themecomponent")] // POST /api/components/themecomponent
        [ProducesResponseType(typeof(ThemeComponentDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ThemeComponentDto>> AddComponentToTheme([FromBody] ThemeComponentDto themeComponentDto)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdRelation = await _componentService.AddComponentToThemeAsync(themeComponentDto);
                return StatusCode(201, createdRelation);
            }
            catch (ArgumentException ex) // Servisten gelen doğrulama hataları
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex) // Zaten var veya DB hatası
            {
                if (ex.Message.Contains("zaten bu temaya atanmış"))
                    return Conflict(ex.Message);
                else
                    return StatusCode(500, $"Tema-Bileşen ilişkisi eklenirken hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Tema-Bileşen ilişkisi eklenirken beklenmedik bir sunucu hatası oluştu: {ex.Message}");
            }
        }

        /// Mevcut bir tema-bileşen ilişkisini günceller.
        /// <response code="200">İlişki başarıyla güncellendi.</response>
        /// <response code="400">Gönderilen ID ile veri uyuşmuyor veya geçersiz veri.</response>
        /// <response code="404">Güncellenecek ilişki bulunamadı.</response>
        /// <response code="500">İlişki güncellenirken sunucu hatası oluştu.</response>
        [HttpPut("themecomponent/{id}")] // PUT /api/components/themecomponent/10
        [ProducesResponseType(typeof(ThemeComponentDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ThemeComponentDto>> UpdateThemeComponent(int id, [FromBody] ThemeComponentDto themeComponentDto)
        {
            if (id != themeComponentDto.Id)
            {
                return BadRequest("URL'deki ID ile gönderilen ilişki ID'si uyuşmuyor.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedRelation = await _componentService.UpdateThemeComponentAsync(themeComponentDto);
                return Ok(updatedRelation);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                 return StatusCode(500, $"Tema-Bileşen ilişkisi güncellenirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Tema-Bileşen ilişkisi güncellenirken beklenmedik bir sunucu hatası oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }

        /// Belirtilen tema-bileşen ilişkisini pasif hale getirir (soft delete).
        /// <response code="204">İlişki başarıyla pasifleştirildi.</response>
        /// <response code="404">Pasifleştirilecek ilişki bulunamadı.</response>
        /// <response code="500">İlişki silinirken sunucu hatası oluştu.</response>
        [HttpDelete("themecomponent/{id}")] // DELETE /api/components/themecomponent/10
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RemoveComponentFromTheme(int id)
        {
            try
            {
                await _componentService.RemoveComponentFromThemeAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex) when (ex.InnerException is KeyNotFoundException)
            {
                return NotFound($"Silinecek tema-bileşen ilişkisi bulunamadı (ID: {id}).");
            }
             catch (InvalidOperationException ex)
            {
                return StatusCode(500, $"Tema-Bileşen ilişkisi silinirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Tema-Bileşen ilişkisi silinirken beklenmedik bir sunucu hatası oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }

        /// Belirtilen siteye ait tüm aktif bileşenleri ve verilerini getirir.
        /// <response code="200">Bileşen verileri başarıyla listelendi.</response>
        /// <response code="400">Geçersiz site ID'si.</response>
        /// <response code="500">Bileşen verileri listelenirken sunucu hatası oluştu.</response>
        [HttpGet("forsite/{siteId}")] // GET /api/components/forsite/1
        [ProducesResponseType(typeof(IEnumerable<SiteComponentDataDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SiteComponentDataDto>>> GetComponentsForSite(int siteId)
        {
            if (siteId <= 0)
            {
                return BadRequest("Geçerli bir site ID'si gereklidir.");
            }

            try
            {
                var components = await _componentService.GetComponentsForSiteAsync(siteId);
                return Ok(components);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Siteye ait bileşen verileri listelenirken hata oluştu (Site ID: {siteId}). Detay: {ex.Message}");
            }
        }

        /// Belirtilen ID'ye sahip aktif site bileşen verisini getirir.
        /// <response code="200">Bileşen verisi başarıyla döndürüldü.</response>
        /// <response code="404">Belirtilen ID'ye sahip bileşen verisi bulunamadı.</response>
        /// <response code="500">Bileşen verisi getirilirken sunucu hatası oluştu.</response>
        [HttpGet("sitedata/{id}")] // GET /api/components/sitedata/25
        [ProducesResponseType(typeof(SiteComponentDataDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<SiteComponentDataDto>> GetComponentData(int id)
        {
             try
            {
                var data = await _componentService.GetComponentDataAsync(id);
                if (data == null)
                {
                    return NotFound($"ID'si {id} olan site bileşen verisi bulunamadı.");
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Site bileşen verisi getirilirken hata oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }

        /// Mevcut bir site bileşen verisini günceller (Sadece 'Data' alanı güncellenir).
        /// <response code="200">Bileşen verisi başarıyla güncellendi.</response>
        /// <response code="400">Gönderilen ID ile veri uyuşmuyor veya geçersiz veri.</response>
        /// <response code="404">Güncellenecek bileşen verisi bulunamadı.</response>
        /// <response code="500">Bileşen verisi güncellenirken sunucu hatası oluştu.</response>
        [HttpPut("sitedata/{id}")] // PUT /api/components/sitedata/25
        [ProducesResponseType(typeof(SiteComponentDataDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<SiteComponentDataDto>> UpdateComponentData(int id, [FromBody] SiteComponentDataDto componentDataDto)
        {
             if (id != componentDataDto.Id)
            {
                if (componentDataDto.Id == null) componentDataDto.Id = id;
                else if (id != componentDataDto.Id) return BadRequest("URL'deki ID ile gönderilen veri ID'si uyuşmuyor.");
            }

            try
            {
                var updatedData = await _componentService.UpdateComponentDataAsync(componentDataDto);
                return Ok(updatedData);
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
                 return StatusCode(500, $"Site bileşen verisi güncellenirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Site bileşen verisi güncellenirken beklenmedik bir sunucu hatası oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }
    }
} 