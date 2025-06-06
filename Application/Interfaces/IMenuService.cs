using new_cms.Application.DTOs.MenuDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.Application.Interfaces
{
    /// Menü yönetimi (TAppMenu) ile ilgili operasyonları tanımlayan arayüz.
    public interface IMenuService
    {

        /// Belirtilen siteye ait en üst seviyedeki aktif menüleri listeler.
        Task<IEnumerable<MenuListDto>> GetMenusBySiteIdAsync(int siteId);


        /// Belirtilen menü ID'sinden başlayarak tüm alt menüleri hiyerarşik olarak getirir.
        Task<MenuTreeDto?> GetMenuByIdAsync(int id);
        

        /// Belirli bir site için tüm menü öğelerini hiyerarşik olarak getirir.
        Task<IEnumerable<MenuTreeDto>> GetMenuTreeBySiteIdAsync(int siteId);


        /// Yeni bir menü öğesi oluşturur.
        Task<MenuDto> CreateMenuAsync(MenuDto menuDto);


        /// Mevcut bir menü öğesini günceller.
        Task<MenuDto> UpdateMenuAsync(MenuDto menuDto);


        /// Belirtilen ID'ye sahip menü öğesini ve tüm alt öğelerini (hiyerarşik olarak) pasif hale getirir (soft delete).
        Task DeleteMenuAsync(int id);

        /// Sistemdeki tüm menüleri listeler.
        Task<IEnumerable<MenuListDto>> GetAllMenusAsync();

        /// Belirtilen üst menüye ait alt menüleri listeler.
        Task<IEnumerable<MenuListDto>> GetMenusByParentIdAsync(int parentId);
    }
} 