using System.Collections.Generic;
using System.Threading.Tasks;
using new_cms.Application.DTOs.ThemeDTOs;
using new_cms.Application.DTOs.ComponentDTOs;
namespace new_cms.Application.Interfaces
{
    public interface IThemeService
    {
        // Tüm temaları listeler
        Task<IEnumerable<ThemeDto>> GetAllThemesAsync();
        
        // Belirli bir temanın detayını getirir
        Task<ThemeDto?> GetThemeByIdAsync(int id);
        
        // Yeni tema oluşturur
        Task<ThemeDto> CreateThemeAsync(ThemeDto themeDto);
        
        // Mevcut temayı günceller
        Task<ThemeDto> UpdateThemeAsync(ThemeDto themeDto);
        
        // Temayı soft delete yapar
        Task DeleteThemeAsync(int id);
        
        // Belirli bir temaya ait bileşenleri listeler
        Task<IEnumerable<ThemeComponentDto>> GetThemeComponentsAsync(int themeId);
        
        // Temaya yeni bileşen ekler
        Task<ThemeComponentDto> AddComponentToThemeAsync(ThemeComponentDto componentDto);
        
        // Tema bileşenini günceller
        Task<ThemeComponentDto> UpdateThemeComponentAsync(ThemeComponentDto componentDto);
        
        // Tema bileşenini kaldırır
        Task RemoveComponentFromThemeAsync(int componentId);
        
        // Temaya eklenebilecek uygun bileşenleri listeler
        Task<IEnumerable<ComponentDto>> GetAvailableComponentsForThemeAsync(int themeId);
        
        // Temanın kullanımda olup olmadığını kontrol eder
        Task<bool> IsThemeInUseAsync(int id);
        
        // Belirli bir tema bileşeninin detaylarını getirir
        Task<ThemeComponentDto?> GetThemeComponentByIdAsync(int componentId);
    }
} 