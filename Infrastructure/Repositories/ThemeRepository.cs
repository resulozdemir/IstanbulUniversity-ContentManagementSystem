using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;
using new_cms.Application.DTOs;

namespace new_cms.Infrastructure.Persistence.Repositories
{
    /// Tema yönetimi için gerekli veritabanı işlemlerini gerçekleştiren repository sınıfı.
    /// Tema oluşturma, düzenleme, listeleme ve tema bileşenlerini yönetme işlemlerini gerçekleştirir.
    public class ThemeRepository : BaseRepository<TAppTheme>, IThemeRepository
    {
        public ThemeRepository(UCmsContext context) : base(context)
        {
        }

        // Tüm temaları getiren metot
        // Tema yönetim panelinde kullanılır
        public async Task<IEnumerable<TAppTheme>> GetAllThemesAsync()
        {
            return await _context.TAppThemes
                .Where(t => t.Isdeleted == 0)
                .ToListAsync();
        }

        // Belirli bir temanın detaylarını getiren metot
        // Tema düzenleme sayfasında kullanılır
        public async Task<TAppTheme> GetThemeByIdAsync(int id)
        {
            return await _context.TAppThemes
                .FirstOrDefaultAsync(t => t.Id == id && t.Isdeleted == 0);
        }

        // Belirli bir temaya ait bileşenleri getiren metot
        // Tema içerik yönetiminde kullanılır
        public async Task<IEnumerable<TAppThemecomponent>> GetThemeComponentsAsync(int themeId)
        {
            return await _context.TAppThemecomponents
                .Where(tc => tc.Themeid == themeId && tc.Isdeleted == 0)
                .ToListAsync();
        }

        // Temanın kullanımda olup olmadığını kontrol eden metot
        // Tema silme işlemi öncesinde kontrol için kullanılır
        public async Task<bool> IsThemeInUseAsync(int id)
        {
            return await _context.TAppSites
                .AnyAsync(s => s.Themeid == id && s.Isdeleted == 0);
        }

        // Belirli bir tema bileşeninin detaylarını getiren metot
        // Tema bileşeni düzenleme sayfasında kullanılır
        public async Task<TAppThemecomponent> GetThemeComponentByIdAsync(int componentId)
        {
            return await _context.TAppThemecomponents
                .FirstOrDefaultAsync(tc => tc.Id == componentId && tc.Isdeleted == 0);
        }

        // Temaya eklenebilecek kullanılabilir bileşenleri getiren metot
        // Tema bileşeni ekleme sayfasında kullanılır
        public async Task<IEnumerable<TAppComponent>> GetAvailableComponentsForThemeAsync(int themeId)
        {
            // Önce temanın mevcut bileşenlerini al
            var themeComponents = await _context.TAppThemecomponents
                .Where(tc => tc.Themeid == themeId && tc.Isdeleted == 0)
                .ToListAsync();

            // Kullanılmayan bileşen tiplerini bul
            var usedComponentIds = themeComponents.Select(tc => tc.Componentid).ToList();

            // Kullanılmayan bileşenleri getir
            return await _context.TAppComponents
                .Where(c => !usedComponentIds.Contains(c.Id) && c.Isdeleted == 0)
                .ToListAsync();
        }
    }
} 