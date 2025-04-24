using new_cms.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.Domain.Interfaces
{
    // Menü verileri için veritabanı işlemlerini tanımlayan interface, IRepository'den türetilmiştir.
    public interface IMenuRepository : IRepository<TAppMenu>
    {
        // Belirli bir menüyü ID ve site ID'ye göre getirir. IRepository'deki GetByIdAsync'tan farklı olarak siteId kontrolü yapar.
        Task<TAppMenu?> GetByIdAsync(int id, int siteId);

        // Belirli bir siteye ait tüm menüleri getirir
        Task<IEnumerable<TAppMenu>> GetBySiteIdAsync(int siteId);

        // Belirli bir siteye ait aktif menüleri getirir
        Task<IEnumerable<TAppMenu>> GetActiveMenusAsync(int siteId);

        // Belirli bir siteye ait kök menüleri (üst menüsü olmayan) getirir
        Task<IEnumerable<TAppMenu>> GetRootMenusAsync(int siteId);
        
        // Belirli bir üst menüye ait alt menüleri getirir
        Task<IEnumerable<TAppMenu>> GetChildMenusAsync(int parentId, int siteId);

        // Belirli bir gruba ait menüleri getirir
        Task<IEnumerable<TAppMenu>> GetMenusByGroupIdAsync(int groupId, int siteId);

        // Belirli bir menüyü siler (soft delete). IRepository'deki DeleteAsync'tan farklı olarak siteId kontrolü yapar.
        Task DeleteAsync(int id, int siteId);

        // Belirli bir menünün varlığını kontrol eder. IRepository'deki ExistsAsync'tan farklı olarak siteId kontrolü yapar.
        Task<bool> ExistsAsync(int id, int siteId);
    }
}
