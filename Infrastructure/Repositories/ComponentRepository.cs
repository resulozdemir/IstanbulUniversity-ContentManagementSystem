using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;
using new_cms.Application.DTOs;

namespace new_cms.Infrastructure.Persistence.Repositories
{
    /// Bileşen yönetimi için gerekli veritabanı işlemlerini gerçekleştiren repository sınıfı.
    /// Bileşen oluşturma, düzenleme, listeleme ve bileşen verilerini yönetme işlemlerini gerçekleştirir.
    public class ComponentRepository : BaseRepository<TAppComponent>, IComponentRepository
    {
        public ComponentRepository(UCmsContext context) : base(context)
        {
        }

        // Tüm bileşenleri DTO formatında getiren metot
        // Bileşen yönetim panelinde kullanılır
        public async Task<IEnumerable<ComponentDto>> GetAllComponentsAsync()
        {
            return await _context.TAppComponents
                .Where(c => c.Isdeleted == 0)
                .Select(c => new ComponentDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Template = c.Template,
                    Style = c.Style,
                    FormJson = c.Formjson,
                    IsDeleted = c.Isdeleted == 1
                })
                .ToListAsync();
        }

        // Belirli bir bileşenin detaylarını getiren metot
        // Bileşen düzenleme sayfasında kullanılır
        public async Task<ComponentDto> GetComponentByIdAsync(int id)
        {
            var component = await _context.TAppComponents
                .FirstOrDefaultAsync(c => c.Id == id && c.Isdeleted == 0);

            if (component == null)
                return null;

            return new ComponentDto
            {
                Id = component.Id,
                Name = component.Name,
                Description = component.Description,
                Template = component.Template,
                Style = component.Style,
                FormJson = component.Formjson,
                IsDeleted = component.Isdeleted == 1
            };
        }

        // Bileşenin herhangi bir temada kullanılıp kullanılmadığını kontrol eden metot
        // Bileşen silme işlemi öncesinde kontrol için kullanılır
        public async Task<bool> IsComponentInUseAsync(int id)
        {
            return await _context.TAppThemecomponents
                .AnyAsync(tc => tc.Componentid == id && tc.Isdeleted == 0);
        }

        // Belirli bir tipteki bileşenleri getiren metot
        // Bileşen filtreleme ve arama işlemlerinde kullanılır
        public async Task<IEnumerable<ComponentDto>> GetComponentsByTypeAsync(string type)
        {
            return await _context.TAppComponents
                .Where(c => c.Name.Contains(type) && c.Isdeleted == 0)
                .Select(c => new ComponentDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Template = c.Template,
                    Style = c.Style,
                    FormJson = c.Formjson,
                    IsDeleted = c.Isdeleted == 1
                })
                .ToListAsync();
        }

        // Belirli bir siteye ait bileşen verilerini getiren metot
        // Site içerik yönetiminde kullanılır
        public async Task<IEnumerable<SiteComponentDataDto>> GetComponentDataBySiteIdAsync(int siteId)
        {
            return await _context.TAppSitecomponentdata
                .Where(cd => cd.Siteid == siteId && cd.Isdeleted == 0)
                .Select(cd => new SiteComponentDataDto
                {
                    Id = cd.Id,
                    SiteId = cd.Siteid,
                    ThemeComponentId = cd.Themecomponentid,
                    Data = cd.Data,
                    Column1 = cd.Column1,
                    Column2 = cd.Column2,
                    Column3 = cd.Column3,
                    Column4 = cd.Column4,
                    CreatedDate = cd.Createddate,
                    CreatedUser = cd.Createduser,
                    ModifiedDate = cd.Modifieddate,
                    ModifiedUser = cd.Modifieduser,
                    IsDeleted = cd.Isdeleted == 1
                })
                .ToListAsync();
        }

        // Belirli bir bileşen verisinin detaylarını getiren metot
        // Bileşen veri düzenleme sayfasında kullanılır
        public async Task<SiteComponentDataDto> GetComponentDataByIdAsync(int dataId)
        {
            var componentData = await _context.TAppSitecomponentdata
                .FirstOrDefaultAsync(cd => cd.Id == dataId && cd.Isdeleted == 0);

            if (componentData == null)
                return null;

            return new SiteComponentDataDto
            {
                Id = componentData.Id,
                SiteId = componentData.Siteid,
                ThemeComponentId = componentData.Themecomponentid,
                Data = componentData.Data,
                Column1 = componentData.Column1,
                Column2 = componentData.Column2,
                Column3 = componentData.Column3,
                Column4 = componentData.Column4,
                CreatedDate = componentData.Createddate,
                CreatedUser = componentData.Createduser,
                ModifiedDate = componentData.Modifieddate,
                ModifiedUser = componentData.Modifieduser,
                IsDeleted = componentData.Isdeleted == 1
            };
        }
    }
} 