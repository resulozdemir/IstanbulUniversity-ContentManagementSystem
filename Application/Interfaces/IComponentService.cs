using new_cms.Application.DTOs.ComponentDTOs;
using new_cms.Application.DTOs.ThemeDTOs; // ThemeComponentDto için
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.Application.Interfaces
{
    /// Bileşen (Component) yönetimi ile ilgili operasyonları tanımlayan arayüz.
    public interface IComponentService
    {
        /// Bir bileşeni belirli bir temaya atar (yeni kayıt oluşturur).
        Task<ThemeComponentDto> AddComponentToThemeAsync(ThemeComponentDto themeComponentDto);

        /// Mevcut bir tema-bileşen ilişkisini günceller.
        Task<ThemeComponentDto> UpdateThemeComponentAsync(ThemeComponentDto themeComponentDto);

        /// Belirtilen tema-bileşen ilişkisini pasif hale getirir (soft delete).
        Task RemoveComponentFromThemeAsync(int themeComponentId);

        /// Belirtilen ID'ye sahip site/sayfa üzerindeki bileşen verisini getirir.
        Task<SiteComponentDataDto?> GetComponentDataAsync(int siteComponentDataId);

        /// Mevcut bir site/sayfa bileşen verisini günceller.
        Task<SiteComponentDataDto> UpdateComponentDataAsync(SiteComponentDataDto componentDataDto);

        /// Belirli bir site için kullanılması gereken tüm bileşenleri ve verilerini getirir.
        Task<IEnumerable<SiteComponentDataDto>> GetComponentsForSiteAsync(int siteId);

        /// Belirtilen ID'ye sahip bileşeni getirir.
        Task<ComponentDto?> GetComponentByIdAsync(int id);
    }
} 