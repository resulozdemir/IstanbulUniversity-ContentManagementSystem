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

        // Component temel CRUD işlemleri

        /// Tüm aktif bileşenleri listeler.
        /// <response code="200">Bileşenler başarıyla listelendi.</response>
        /// <response code="500">Bileşenler listelenirken sunucu hatası oluştu.</response>
        [HttpGet] // GET /api/components
        [ProducesResponseType(typeof(IEnumerable<ComponentDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ComponentDto>>> GetAllComponents()
        {
            try
            {
                var components = await _componentService.GetAllComponentsAsync();
                return Ok(components);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bileşenler listelenirken bir hata oluştu: {ex.Message}");
            }
        }

        /// Belirtilen ID'ye sahip bileşeni getirir.
        /// <response code="200">Bileşen başarıyla döndürüldü.</response>
        /// <response code="404">Belirtilen ID'ye sahip bileşen bulunamadı.</response>
        /// <response code="500">Bileşen getirilirken sunucu hatası oluştu.</response>
        [HttpGet("{id}")] // GET /api/components/5
        [ProducesResponseType(typeof(ComponentDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ComponentDto>> GetComponentById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Geçerli bir bileşen ID'si gereklidir.");
            }

            try
            {
                var component = await _componentService.GetComponentByIdAsync(id);
                if (component == null)
                {
                    return NotFound($"ID'si {id} olan bileşen bulunamadı.");
                }
                return Ok(component);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bileşen getirilirken bir hata oluştu (ID: {id}): {ex.Message}");
            }
        }

        /// Yeni bir bileşen oluşturur.
        /// <response code="201">Bileşen başarıyla oluşturuldu.</response>
        /// <response code="400">Geçersiz veri gönderildi.</response>
        /// <response code="409">Aynı isimde bir bileşen zaten mevcut (Conflict).</response>
        /// <response code="500">Bileşen oluşturulurken sunucu hatası oluştu.</response>
        [HttpPost] // POST /api/components
        [ProducesResponseType(typeof(ComponentDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ComponentDto>> CreateComponent([FromBody] ComponentDto componentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdComponent = await _componentService.CreateComponentAsync(componentDto);
                return StatusCode(201, createdComponent);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("zaten mevcut"))
                    return Conflict(ex.Message);
                else
                    return StatusCode(500, $"Bileşen oluşturulurken hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bileşen oluşturulurken beklenmedik bir sunucu hatası oluştu: {ex.Message}");
            }
        }

        /// Mevcut bir bileşeni günceller.
        /// <response code="200">Bileşen başarıyla güncellendi.</response>
        /// <response code="400">Gönderilen ID ile veri uyuşmuyor veya geçersiz veri.</response>
        /// <response code="404">Güncellenecek bileşen bulunamadı.</response>
        /// <response code="409">Aynı isimde başka bir bileşen mevcut (Conflict).</response>
        /// <response code="500">Bileşen güncellenirken sunucu hatası oluştu.</response>
        [HttpPut("{id}")] // PUT /api/components/5
        [ProducesResponseType(typeof(ComponentDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ComponentDto>> UpdateComponent(int id, [FromBody] ComponentDto componentDto)
        {
            if (id != componentDto.Id)
            {
                return BadRequest("URL'deki ID ile gönderilen bileşen ID'si uyuşmuyor.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedComponent = await _componentService.UpdateComponentAsync(componentDto);
                return Ok(updatedComponent);
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
                if (ex.Message.Contains("başka bir aktif bileşen mevcut"))
                    return Conflict(ex.Message);
                else
                    return StatusCode(500, $"Bileşen güncellenirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bileşen güncellenirken beklenmedik bir sunucu hatası oluştu (ID: {id}). Detay: {ex.Message}");
            }
        }

        /// Belirtilen ID'ye sahip bileşeni pasif hale getirir (soft delete).
        /// <response code="204">Bileşen başarıyla pasifleştirildi.</response>
        /// <response code="400">Geçersiz ID.</response>
        /// <response code="500">Bileşen silinirken sunucu hatası oluştu.</response>
        [HttpDelete("{id}")] // DELETE /api/components/5
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteComponent(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Geçerli bir bileşen ID'si gereklidir.");
            }

            try
            {
                await _componentService.DeleteComponentAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, $"Bileşen silinirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bileşen silinirken beklenmedik bir sunucu hatası oluştu (ID: {id}). Detay: {ex.Message}");
            }
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

        // Tema-Bileşen ilişki işlemleri

        /// Tüm aktif tema-bileşen ilişkilerini listeler.
        /// <response code="200">Tema-bileşen ilişkileri başarıyla listelendi.</response>
        /// <response code="500">Tema-bileşen ilişkileri listelenirken sunucu hatası oluştu.</response>
        [HttpGet("themecomponent")] // GET /api/components/themecomponent
        [ProducesResponseType(typeof(IEnumerable<ThemeComponentDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ThemeComponentDto>>> GetAllThemeComponents()
        {
            try
            {
                var themeComponents = await _componentService.GetAllThemeComponentsAsync();
                return Ok(themeComponents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Tema-bileşen ilişkileri listelenirken bir hata oluştu: {ex.Message}");
            }
        }

        /// Belirtilen ID'ye sahip tema-bileşen ilişkisini getirir.
        /// <response code="200">Tema-bileşen ilişkisi başarıyla döndürüldü.</response>
        /// <response code="404">Belirtilen ID'ye sahip tema-bileşen ilişkisi bulunamadı.</response>
        /// <response code="500">Tema-bileşen ilişkisi getirilirken sunucu hatası oluştu.</response>
        [HttpGet("themecomponent/{id}")] // GET /api/components/themecomponent/10
        [ProducesResponseType(typeof(ThemeComponentDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ThemeComponentDto>> GetThemeComponentById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Geçerli bir tema-bileşen ID'si gereklidir.");
            }

            try
            {
                var themeComponent = await _componentService.GetThemeComponentByIdAsync(id);
                if (themeComponent == null)
                {
                    return NotFound($"ID'si {id} olan tema-bileşen ilişkisi bulunamadı.");
                }
                return Ok(themeComponent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Tema-bileşen ilişkisi getirilirken bir hata oluştu (ID: {id}): {ex.Message}");
            }
        }
    }
} 