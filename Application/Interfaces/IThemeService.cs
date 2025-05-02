using new_cms.Application.DTOs.ThemeDTOs;
using new_cms.Application.DTOs.ComponentDTOs; // ThemeComponentDto için
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.Application.Interfaces
{
    
    /// Tema yönetimi ile ilgili operasyonları tanımlayan arayüz.
    public interface IThemeService
    {
        /// Sistemdeki tüm aktif temaları listeler.
        Task<IEnumerable<ThemeDto>> GetAllThemesAsync();

        /// Belirtilen ID'ye sahip aktif temayı getirir.
        Task<ThemeDto?> GetThemeByIdAsync(int id);

        /// Yeni bir tema oluşturur.
        Task<ThemeDto> CreateThemeAsync(ThemeDto themeDto);

        /// Mevcut bir temanın bilgilerini günceller.
        Task<ThemeDto> UpdateThemeAsync(ThemeDto themeDto);

        /// Belirtilen ID'ye sahip temayı pasif hale getirir (soft delete).
        Task DeleteThemeAsync(int id);

    }
} 