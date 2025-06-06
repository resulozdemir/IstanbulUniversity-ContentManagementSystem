using Microsoft.AspNetCore.Mvc;
using new_cms.Application.DTOs.ThemeDTOs;
using new_cms.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.WebApi.Controllers
{
    /// Temalarla ilgili API işlemlerini yönetir.
    [ApiController]
    [Route("api/[controller]")]
    public class ThemesController : ControllerBase
    {
        private readonly IThemeService _themeService;

        public ThemesController(IThemeService themeService)
        {
            _themeService = themeService;
        }


        /// Tüm aktif temaları listeler.
        /// <response code="200">Temaların listesi başarıyla döndürüldü.</response>
        /// <response code="500">Temaları listelerken sunucu hatası oluştu.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ThemeDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ThemeDto>>> GetAllThemes()
        {
            try
            {
                var themes = await _themeService.GetAllThemesAsync();
                return Ok(themes);
            }
            catch (Exception)
            {
                return StatusCode(500, "Temaları alırken sunucu hatası oluştu.");
            }
        }


        /// Belirtilen ID'ye sahip aktif temayı getirir.
        /// <response code="200">Tema başarıyla döndürüldü.</response>
        /// <response code="404">Belirtilen ID'ye sahip tema bulunamadı.</response>
        /// <response code="500">Tema getirilirken sunucu hatası oluştu.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ThemeDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ThemeDto>> GetThemeById(int id)
        {
            try
            {
                var theme = await _themeService.GetThemeByIdAsync(id);
                if (theme == null)
                {
                    return NotFound($"ID'si {id} olan tema bulunamadı.");
                }
                return Ok(theme);
            }
            catch (Exception)
            {
                return StatusCode(500, $"Tema getirilirken bir hata oluştu (ID: {id}).");
            }
        }


        /// Yeni bir tema oluşturur.
        /// <response code="201">Tema başarıyla oluşturuldu.</response>
        /// <response code="400">Geçersiz tema verisi gönderildi.</response>
        /// <response code="500">Tema oluşturulurken sunucu hatası oluştu.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ThemeDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ThemeDto>> CreateTheme([FromBody] ThemeDto themeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdTheme = await _themeService.CreateThemeAsync(themeDto);
                
                return CreatedAtAction(nameof(GetThemeById), new { id = createdTheme.Id }, createdTheme);
            }
            catch (InvalidOperationException ex) // Servisten beklenen özel hata türü
            {
                 return StatusCode(500, $"Tema oluşturulurken bir hata oluştu: {ex.Message}");
            }
            catch (Exception)
            {
                return StatusCode(500, "Tema oluşturulurken beklenmedik bir sunucu hatası oluştu.");
            }
        }


        /// Mevcut bir temayı günceller.
        /// <response code="200">Tema başarıyla güncellendi.</response>
        /// <response code="400">Gönderilen ID ile tema verisi uyuşmuyor veya geçersiz veri.</response>
        /// <response code="404">Güncellenecek tema bulunamadı.</response>
        /// <response code="500">Tema güncellenirken sunucu hatası oluştu.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ThemeDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ThemeDto>> UpdateTheme(int id, [FromBody] ThemeDto themeDto)
        {
            if (id != themeDto.Id)
            {
                return BadRequest("URL'deki ID ile gönderilen tema ID'si uyuşmuyor.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedTheme = await _themeService.UpdateThemeAsync(themeDto);
                 return Ok(updatedTheme); // Güncellenmiş temayı döndür
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentNullException ex) // Servisten gelebilecek null ID hatası
            {
                 return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex) // Servisten beklenen özel hata türü
            {
                 return StatusCode(500, $"Tema güncellenirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception)
            {
                return StatusCode(500, $"Tema güncellenirken beklenmedik bir sunucu hatası oluştu (ID: {id}).");
            }
        }


        /// Belirtilen ID'ye sahip temayı pasif hale getirir (soft delete).
        /// <response code="204">Tema başarıyla pasifleştirildi.</response>
        /// <response code="404">Pasifleştirilecek tema bulunamadı.</response>
        /// <response code="500">Tema silinirken sunucu hatası oluştu.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteTheme(int id)
        {
            try
            {
                await _themeService.DeleteThemeAsync(id);
                return NoContent(); 
            }
            catch (KeyNotFoundException ex) 
            {
                 return NotFound($"Silinecek tema bulunamadı (ID: {id}). Detay: {ex.Message}");
            }
            catch (InvalidOperationException ex) 
            {
                return StatusCode(500, $"Tema silinirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception)
            {
                return StatusCode(500, $"Tema silinirken beklenmedik bir sunucu hatası oluştu (ID: {id}).");
            }
        }
    }
} 