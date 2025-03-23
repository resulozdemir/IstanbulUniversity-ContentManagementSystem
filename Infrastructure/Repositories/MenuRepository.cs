using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;

namespace new_cms.Infrastructure.Persistence.Repositories
{
    // Menü yönetimi için veritabanı işlemlerini gerçekleştiren repository sınıfı
    public class MenuRepository : BaseRepository<TAppMenu>, IMenuRepository
    {
        public MenuRepository(UCmsContext context) : base(context)
        {
        }

        // Menü ID ve Site ID'ye göre tek bir menü getir
        public async Task<TAppMenu> GetByIdAsync(int id, int siteId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(m => m.Id == id && m.Siteid == siteId && m.Isdeleted == 0);
        }

        // Site ID'ye göre aktif menüleri getir
        public async Task<IEnumerable<TAppMenu>> GetActiveMenusAsync(int siteId)
        {
            return await _dbSet
                .Where(m => m.Siteid == siteId && m.Isdeleted == 0 && m.Status == 1)
                .OrderBy(m => m.Menuorder)
                .ToListAsync();
        }

        // Site ID'ye göre tüm menüleri getir
        public async Task<IEnumerable<TAppMenu>> GetBySiteIdAsync(int siteId)
        {
            return await _dbSet
                .Where(m => m.Siteid == siteId && m.Isdeleted == 0)
                .OrderBy(m => m.Menuorder)
                .ToListAsync();
        }

        // Kök menüleri getir (parent ID'si olmayan menüler)
        public async Task<IEnumerable<TAppMenu>> GetRootMenusAsync(int siteId)
        {
            return await _dbSet
                .Where(m => m.Siteid == siteId && m.Isdeleted == 0 && m.Parentid == null)
                .OrderBy(m => m.Menuorder)
                .ToListAsync();
        }

        // Belirli bir üst menüye bağlı alt menüleri getir
        public async Task<IEnumerable<TAppMenu>> GetChildMenusAsync(int parentId, int siteId)
        {
            return await _dbSet
                .Where(m => m.Siteid == siteId && m.Isdeleted == 0 && m.Parentid == parentId)
                .OrderBy(m => m.Menuorder)
                .ToListAsync();
        }

        // Menü grubu ID'sine göre menüleri getir
        public async Task<IEnumerable<TAppMenu>> GetMenusByGroupIdAsync(int groupId, int siteId)
        {
            return await _dbSet
                .Where(m => m.Siteid == siteId && m.Isdeleted == 0 && m.Groupid == groupId)
                .OrderBy(m => m.Menuorder)
                .ToListAsync();
        }

        // Site ID'ye göre toplam menü sayısını getir
        public async Task<int> GetTotalCountAsync(int siteId)
        {
            return await _dbSet
                .CountAsync(m => m.Siteid == siteId && m.Isdeleted == 0);
        }

        // Menü ID ve Site ID'ye göre menü varlığını kontrol et
        public async Task<bool> ExistsAsync(int id, int siteId)
        {
            return await _dbSet.AnyAsync(m => m.Id == id && m.Siteid == siteId && m.Isdeleted == 0);
        }

        // Menü ID ve Site ID'ye göre silme işlemi (Soft Delete)
        public async Task DeleteAsync(int id, int siteId)
        {
            var menu = await GetByIdAsync(id, siteId);
            if (menu != null)
            {
                menu.Isdeleted = 1;
                await UpdateAsync(menu);
            }
        }
    }
}
