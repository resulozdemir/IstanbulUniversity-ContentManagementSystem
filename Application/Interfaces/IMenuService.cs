using System.Collections.Generic;
using System.Threading.Tasks;
using new_cms.Application.DTOs.MenuDTOs;

namespace new_cms.Application.Interfaces
{
    public interface IMenuService
    {
        // Tüm menüleri listeler
        Task<IEnumerable<MenuListDto>> GetAllMenusAsync();
        
        // Belirli bir menü detayını getirir
        Task<MenuDetailDto?> GetMenuByIdAsync(int id);
        
        // Yeni menü oluşturur
        Task<MenuDto> CreateMenuAsync(MenuDto menuDto);
        
        // Mevcut menüyü günceller
        Task<MenuDto> UpdateMenuAsync(MenuDto menuDto);
        
        // Menüyü soft delete yapar
        Task DeleteMenuAsync(int id);
        
        // Site ID'ye göre menüleri listeler
        Task<IEnumerable<MenuListDto>> GetMenusBySiteIdAsync(int siteId);
        
        // Menü öğelerini listeler
        Task<IEnumerable<MenuItemDto>> GetMenuItemsByMenuIdAsync(int menuId);
        
        // Yeni menü öğesi oluşturur
        Task<MenuItemDto> CreateMenuItemAsync(MenuItemDto menuItemDto);
        
        // Menü öğesini günceller
        Task<MenuItemDto> UpdateMenuItemAsync(MenuItemDto menuItemDto);
        
        // Menü öğesini siler
        Task DeleteMenuItemAsync(int id);
        
        // Sayfalı ve filtrelenmiş menü listesi döndürür
        Task<(IEnumerable<MenuListDto> Items, int TotalCount)> GetPagedMenusAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true);
            
        // Ana menü öğelerini listeler (üst öğesi olmayan)
        Task<IEnumerable<MenuItemDto>> GetMainMenuItemsAsync(int menuId);
        
        // Alt menü öğelerini listeler
        Task<IEnumerable<MenuItemDto>> GetSubMenuItemsAsync(int parentId);
        
        // Menü ağacını oluşturur (hiyerarşik yapı)
        Task<IEnumerable<MenuTreeDto>> GetMenuTreeAsync(int menuId);
        
        // Aktif (yayında) olan menüleri listeler
        Task<IEnumerable<MenuListDto>> GetActiveMenusAsync();
    }
} 