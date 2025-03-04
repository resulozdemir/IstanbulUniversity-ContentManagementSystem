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

        // Tüm temaları DTO formatında getiren metot
        // Tema yönetim panelinde kullanılır
        public async Task<IEnumerable<ThemeDto>> GetAllThemesAsync()
        {
            return await _context.TAppThemes
                .Where(t => t.Isdeleted == 0)
                .Select(t => new ThemeDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Header = t.Header,
                    Footer = t.Footer,
                    IsDeleted = t.Isdeleted == 1
                })
                .ToListAsync();
        }

        // Belirli bir temanın detaylarını getiren metot
        // Tema düzenleme sayfasında kullanılır
        public async Task<ThemeDto> GetThemeByIdAsync(int id)
        {
            var theme = await _context.TAppThemes
                .FirstOrDefaultAsync(t => t.Id == id && t.Isdeleted == 0);

            if (theme == null)
                return null;

            return new ThemeDto
            {
                Id = theme.Id,
                Name = theme.Name,
                Header = theme.Header,
                Footer = theme.Footer,
                IsDeleted = theme.Isdeleted == 1
            };
        }

        // Belirli bir temaya ait bileşenleri getiren metot
        // Tema içerik yönetiminde kullanılır
        public async Task<IEnumerable<ThemeComponentDto>> GetThemeComponentsAsync(int themeId)
        {
            return await _context.TAppThemecomponents
                .Where(tc => tc.Themeid == themeId && tc.Isdeleted == 0)
                .Select(tc => new ThemeComponentDto
                {
                    Id = tc.Id,
                    ThemeId = tc.Themeid ?? 0,
                    ComponentId = tc.Componentid,
                    Name = tc.Name,
                    Description = tc.Description,
                    Template = tc.Template,
                    Style = tc.Style,
                    Javascript = tc.Javascript,
                    FormJson = tc.Formjson,
                    FormHtml = tc.Formhtml,
                    FormJs = tc.Formjs,
                    IsDeleted = tc.Isdeleted == 1
                })
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
        public async Task<ThemeComponentDto> GetThemeComponentByIdAsync(int componentId)
        {
            var component = await _context.TAppThemecomponents
                .FirstOrDefaultAsync(tc => tc.Id == componentId && tc.Isdeleted == 0);

            if (component == null)
                return null;

            return new ThemeComponentDto
            {
                Id = component.Id,
                ThemeId = component.Themeid ?? 0,
                ComponentId = component.Componentid,
                Name = component.Name,
                Description = component.Description,
                Template = component.Template,
                Style = component.Style,
                Javascript = component.Javascript,
                FormJson = component.Formjson,
                FormHtml = component.Formhtml,
                FormJs = component.Formjs,
                IsDeleted = component.Isdeleted == 1
            };
        }

        // Temaya eklenebilecek kullanılabilir bileşenleri getiren metot
        // Tema bileşeni ekleme sayfasında kullanılır
        public async Task<IEnumerable<ThemeComponentDto>> GetAvailableComponentsForThemeAsync(int themeId)
        {
            // Önce temanın mevcut bileşenlerini al
            var themeComponents = await GetThemeComponentsAsync(themeId);

            // Kullanılmayan bileşen tiplerini bul
            var usedComponentIds = themeComponents.Select(tc => tc.ComponentId).ToList();

            // Kullanılmayan bileşenleri getir
            var availableComponents = await _context.TAppComponents
                .Where(c => !usedComponentIds.Contains(c.Id) && c.Isdeleted == 0)
                .Select(c => new ThemeComponentDto
                {
                    ComponentId = c.Id,
                    ThemeId = themeId,
                    Name = c.Name,
                    Description = c.Description,
                    Template = "", // Yeni bileşen olduğu için boş
                    Style = "",
                    Javascript = "",
                    FormJson = c.Formjson,
                    FormHtml = "", // Component'te bu alan yok
                    FormJs = "", // Component'te bu alan yok
                    IsDeleted = false
                })
                .ToListAsync();

            return availableComponents;
        }
    }
} 