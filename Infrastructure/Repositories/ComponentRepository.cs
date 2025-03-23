using System;
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

        // Tüm bileşenleri getiren metot
        // Bileşen yönetim panelinde kullanılır
        public async Task<IEnumerable<TAppComponent>> GetAllComponentsAsync()
        {
            return await _context.TAppComponents
                .Where(c => c.Isdeleted == 0)
                .ToListAsync();
        }

        // Belirli bir bileşenin detaylarını getiren metot
        // Bileşen düzenleme sayfasında kullanılır
        public async Task<TAppComponent?> GetComponentByIdAsync(int id)
        {
            return await _context.TAppComponents
                .FirstOrDefaultAsync(c => c.Id == id && c.Isdeleted == 0);
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
        public async Task<IEnumerable<TAppComponent>> GetComponentsByTypeAsync(string type)
        {
            return await _context.TAppComponents
                .Where(c => c.Name.Contains(type) && c.Isdeleted == 0)
                .ToListAsync();
        }

        // Belirli bir siteye ait bileşen verilerini getiren metot
        // Site içerik yönetiminde kullanılır
        public async Task<IEnumerable<TAppSitecomponentdata>> GetComponentDataBySiteIdAsync(int siteId)
        {
            return await _context.TAppSitecomponentdata
                .Where(cd => cd.Siteid == siteId && cd.Isdeleted == 0)
                .ToListAsync();
        }

        // Belirli bir bileşen verisinin detaylarını getiren metot
        // Bileşen veri düzenleme sayfasında kullanılır
        public async Task<TAppSitecomponentdata?> GetComponentDataByIdAsync(int dataId)
        {
            return await _context.TAppSitecomponentdata
                .FirstOrDefaultAsync(cd => cd.Id == dataId && cd.Isdeleted == 0);
        }
    }
} 