using System.Collections.Generic;
using System.Threading.Tasks;
using new_cms.Domain.Entities;
using new_cms.Application.DTOs;

namespace new_cms.Domain.Interfaces
{
    public interface IThemeRepository : IRepository<TAppTheme> //telamaların yonetimi
    {
        // Tüm temaları entity formatında listeler. Tema yönetim sayfası için gerekli.
        Task<IEnumerable<TAppTheme>> GetAllThemesAsync();

        // Belirtilen tema ID'sine ait detayları döndürür. Tema düzenleme sayfası için gerekli.
        Task<TAppTheme> GetThemeByIdAsync(int id);

        // Belirtilen temaya ait bileşenleri listeler. Tema içerik yönetimi için gerekli.
        Task<IEnumerable<TAppThemecomponent>> GetThemeComponentsAsync(int themeId);

        // Temanın herhangi bir sitede kullanılıp kullanılmadığını kontrol eder. Tema silme işlemi öncesi kontrol için.
        Task<bool> IsThemeInUseAsync(int id);

        // Belirtilen tema bileşeninin detaylarını döndürür. Bileşen düzenleme sayfası için gerekli.
        Task<TAppThemecomponent> GetThemeComponentByIdAsync(int componentId);

        // Temaya eklenebilecek kullanılabilir bileşenleri listeler. Tema bileşen ekleme sayfası için gerekli.
        Task<IEnumerable<TAppComponent>> GetAvailableComponentsForThemeAsync(int themeId);
    }
} 