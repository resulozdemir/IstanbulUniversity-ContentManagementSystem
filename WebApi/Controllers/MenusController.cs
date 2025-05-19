using Microsoft.AspNetCore.Mvc;
using new_cms.Application.DTOs.MenuDTOs;
using new_cms.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.WebApi.Controllers
{
    /// Menülerle ilgili API işlemlerini yönetir.
    [ApiController]
    [Route("api/menus")]
    public class MenusController : ControllerBase
    {
        private readonly IMenuService _menuService;
        public MenusController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        /// Sistemdeki tüm menüleri listeler.
        /// <response code="200">Tüm menüler başarıyla döndürüldü.</response>
        /// <response code="500">Menüler listelenirken sunucu hatası oluştu.</response>
        [HttpGet("all")] // GET /api/menus/all
        [ProducesResponseType(typeof(IEnumerable<MenuListDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<MenuListDto>>> GetAllMenus()
        {
            try
            {
                var allMenus = await _menuService.GetAllMenusAsync();
                return Ok(allMenus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Tüm menüler listelenirken bir hata oluştu: {ex.Message}");
            }
        }

        /// Belirtilen üst menüye ait alt menüleri listeler.
        /// <response code="200">Alt menüler başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz üst menü ID'si.</response>
        /// <response code="500">Alt menüler listelenirken sunucu hatası oluştu.</response>
        [HttpGet("parent/{parentId}")] // GET /api/menus/parent/5
        [ProducesResponseType(typeof(IEnumerable<MenuListDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<MenuListDto>>> GetMenusByParentId(int parentId)
        {
            if (parentId <= 0)
            {
                return BadRequest("Geçerli bir üst menü ID'si gereklidir.");
            }

            try
            {
                var childMenus = await _menuService.GetMenusByParentIdAsync(parentId);
                return Ok(childMenus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Alt menüler listelenirken bir hata oluştu (Üst menü ID: {parentId}): {ex.Message}");
            }
        }

        /// Belirtilen siteye ait en üst seviyedeki aktif menüleri listeler.
        /// <response code="200">Menü listesi başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz site ID'si.</response>
        /// <response code="500">Menüleri listelerken sunucu hatası oluştu.</response>
        [HttpGet] // Örn: GET /api/menus?siteId=1
        [ProducesResponseType(typeof(IEnumerable<MenuListDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<MenuListDto>>> GetMenusBySiteId([FromQuery] int siteId)
        {
            if (siteId <= 0)
            {
                return BadRequest("Geçerli bir site ID'si gereklidir.");
            }

            try
            {
                var menus = await _menuService.GetMenusBySiteIdAsync(siteId);
                return Ok(menus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Siteye ait menüler listelenirken hata oluştu (Site ID: {siteId}).");
            }
        }


        /// Belirtilen menü ID'sinden başlayarak tüm alt menüleri hiyerarşik olarak getirir.
        /// <response code="200">Menü ağacı başarıyla döndürüldü.</response>
        /// <response code="404">Belirtilen ID'ye sahip menü bulunamadı.</response>
        /// <response code="500">Menü ağacı getirilirken sunucu hatası oluştu.</response>
        [HttpGet("{id}")] // Örn: GET /api/menus/5
        [ProducesResponseType(typeof(MenuTreeDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<MenuTreeDto>> GetMenuById(int id)
        {
            try
            {
                var menuTree = await _menuService.GetMenuByIdAsync(id);
                if (menuTree == null)
                {
                    return NotFound($"ID'si {id} olan menü bulunamadı.");
                }
                return Ok(menuTree);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Menü ağacı getirilirken hata oluştu (ID: {id}).");
            }
        }


        /// Belirtilen menü ID'sinden başlayarak tüm alt menüleri hiyerarşik olarak getirir.
        /// <response code="200">Menü ağacı başarıyla döndürüldü.</response>
        /// <response code="400">Geçersiz site ID'si.</response>
        /// <response code="500">Menü ağacı getirilirken sunucu hatası oluştu.</response>
        [HttpGet("tree")] // Örn: GET /api/menus/tree?siteId=1
        [ProducesResponseType(typeof(IEnumerable<MenuTreeDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<MenuTreeDto>>> GetMenuTreeBySiteId([FromQuery] int siteId)
        {
             if (siteId <= 0)
            {
                return BadRequest("Geçerli bir site ID'si gereklidir.");
            }
            try
            {
                var menuTree = await _menuService.GetMenuTreeBySiteIdAsync(siteId);
                return Ok(menuTree);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Site için menü ağacı getirilirken hata oluştu (Site ID: {siteId}).");
            }
        }


        /// Yeni bir menü öğesi oluşturur.
        /// <response code="201">Menü başarıyla oluşturuldu.</response>
        /// <response code="400">Geçersiz menü verisi gönderildi (örn: eksik SiteId, Name).</response>
        /// <response code="500">Menü oluşturulurken sunucu hatası oluştu.</response>
        [HttpPost] // POST /api/menus
        [ProducesResponseType(typeof(MenuDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<MenuDto>> CreateMenu([FromBody] MenuDto menuDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdMenu = await _menuService.CreateMenuAsync(menuDto);
                
                return CreatedAtAction(nameof(GetMenuById), new { id = createdMenu.Id }, createdMenu);
            }
             catch (ArgumentException ex) // Servisten beklenen doğrulama hataları
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex) // Servisten beklenen diğer hatalar
            {
                 return StatusCode(500, $"Menü oluşturulurken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Menü oluşturulurken beklenmedik bir sunucu hatası oluştu.");
            }
        }


        /// Mevcut bir menü öğesini günceller.
        /// <response code="200">Menü başarıyla güncellendi.</response>
        /// <response code="400">Gönderilen ID ile menü verisi uyuşmuyor veya geçersiz veri.</response>
        /// <response code="404">Güncellenecek menü bulunamadı.</response>
        /// <response code="500">Menü güncellenirken sunucu hatası oluştu.</response>
        [HttpPut("{id}")] // PUT /api/menus/5
        [ProducesResponseType(typeof(MenuDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<MenuDto>> UpdateMenu(int id, [FromBody] MenuDto menuDto)
        {
            if (id != menuDto.Id)
            {
                return BadRequest("URL'deki ID ile gönderilen menü ID'si uyuşmuyor.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedMenu = await _menuService.UpdateMenuAsync(menuDto);
                return Ok(updatedMenu);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                 return BadRequest(ex.Message);
            }
             catch (InvalidOperationException ex)
            {
                 return StatusCode(500, $"Menü güncellenirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Menü güncellenirken beklenmedik bir sunucu hatası oluştu (ID: {id}).");
            }
        }


        /// Belirtilen ID'ye sahip menü öğesini ve tüm alt öğelerini pasif hale getirir (soft delete).
        /// <response code="204">Menü ve alt öğeleri başarıyla pasifleştirildi.</response>
        /// <response code="404">Pasifleştirilecek menü bulunamadı.</response>
        /// <response code="500">Menü silinirken sunucu hatası oluştu.</response>
        [HttpDelete("{id}")] // DELETE /api/menus/5
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            try
            {
                await _menuService.DeleteMenuAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, $"Menü silinirken bir hata oluştu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Menü silinirken beklenmedik bir sunucu hatası oluştu (ID: {id}).");
            }
        }
    }
} 