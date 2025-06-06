using new_cms.Application.DTOs.ComponentDTOs;
using new_cms.Application.DTOs.ThemeDTOs; // ThemeComponentDto için
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.Application.Interfaces
{
    /// Bileşen (Component) yönetimi ile ilgili operasyonları tanımlayan arayüz.
    public interface IComponentService
    {
        // Component temel CRUD işlemleri
        /// Yeni bir bileşen oluşturur.
        Task<ComponentDto> CreateComponentAsync(ComponentDto componentDto);

        /// Mevcut bir bileşeni günceller.
        Task<ComponentDto> UpdateComponentAsync(ComponentDto componentDto);

        /// Belirtilen ID'ye sahip bileşeni pasif hale getirir (soft delete).
        Task DeleteComponentAsync(int id);

        /// Tüm aktif bileşenleri listeler.
        Task<IEnumerable<ComponentDto>> GetAllComponentsAsync();

        /// Belirtilen ID'ye sahip bileşeni getirir.
        Task<ComponentDto?> GetComponentByIdAsync(int id);

        // Tema-Bileşen ilişki işlemleri
        /// Bir bileşeni belirli bir temaya atar (yeni kayıt oluşturur).
        Task<ThemeComponentDto> AddComponentToThemeAsync(ThemeComponentDto themeComponentDto);

        /// Mevcut bir tema-bileşen ilişkisini günceller.
        Task<ThemeComponentDto> UpdateThemeComponentAsync(ThemeComponentDto themeComponentDto);

        /// Belirtilen tema-bileşen ilişkisini pasif hale getirir (soft delete).
        Task RemoveComponentFromThemeAsync(int themeComponentId);

        /// Tüm aktif tema-bileşen ilişkilerini listeler.
        Task<IEnumerable<ThemeComponentDto>> GetAllThemeComponentsAsync();

        /// Belirtilen ID'ye sahip tema-bileşen ilişkisini getirir.
        Task<ThemeComponentDto?> GetThemeComponentByIdAsync(int themeComponentId);

        // Site Bileşen Veri işlemleri
        /// Belirtilen ID'ye sahip site/sayfa üzerindeki bileşen verisini getirir.
        Task<SiteComponentDataDto?> GetComponentDataAsync(int siteComponentDataId);

        /// Mevcut bir site/sayfa bileşen verisini günceller.
        Task<SiteComponentDataDto> UpdateComponentDataAsync(SiteComponentDataDto componentDataDto);

        /// Belirli bir site için kullanılması gereken tüm bileşenleri ve verilerini getirir.
        Task<IEnumerable<SiteComponentDataDto>> GetComponentsForSiteAsync(int siteId);
    }
} 