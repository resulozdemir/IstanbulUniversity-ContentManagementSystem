using new_cms.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.Application.Interfaces
{
    public interface IMenuRepository  // Menü verileri için veritabanı işlemlerini tanımlayan interface
    {
        // Belirli bir menüyü ID ve site ID'ye göre getirir
        Task<TAppMenu> GetByIdAsync(int id, int siteId);

        // Tüm menüleri getirir
        Task<IEnumerable<TAppMenu>> GetAllAsync();

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

        // Yeni bir menü ekler
        Task<TAppMenu> AddAsync(TAppMenu menu);

        // Mevcut bir menüyü günceller
        Task<TAppMenu> UpdateAsync(TAppMenu menu);

        // Belirli bir menüyü siler (soft delete)
        Task DeleteAsync(int id, int siteId);

        // Belirli bir menünün varlığını kontrol eder
        Task<bool> ExistsAsync(int id, int siteId);

        // Belirli bir siteye ait toplam menü sayısını getirir
        Task<int> GetTotalCountAsync(int siteId);
    }
}
