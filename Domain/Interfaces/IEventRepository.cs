using new_cms.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.Domain.Interfaces
{
    // Etkinlik verileri için veritabanı işlemlerini tanımlayan interface, IRepository'den türetilmiştir.
    public interface IEventRepository : IRepository<TAppEvent>
    {
        // Belirli bir etkinliği ID ve site ID'ye göre getirir. IRepository'deki GetByIdAsync'tan farklı olarak siteId kontrolü yapar.
        Task<TAppEvent?> GetByIdAsync(int id, int siteId);

        // Belirli bir siteye ait tüm etkinlikleri getirir
        Task<IEnumerable<TAppEvent>> GetBySiteIdAsync(int siteId);

        // Belirli bir siteye ait aktif etkinlikleri getirir
        Task<IEnumerable<TAppEvent>> GetActiveEventsAsync(int siteId);

        // Belirli bir etkinliği siler (soft delete). IRepository'deki DeleteAsync'tan farklı olarak siteId kontrolü yapar.
        Task DeleteAsync(int id, int siteId);
        
        // Belirli bir siteye ait en son etkinlikleri belirtilen sayıda getirir
        Task<IEnumerable<TAppEvent>> GetLatestEventsAsync(int siteId, int count);

        // Belirli bir etkinliğin varlığını kontrol eder. IRepository'deki ExistsAsync'tan farklı olarak siteId kontrolü yapar.
        Task<bool> ExistsAsync(int id, int siteId);

        // Belirli bir siteye ait toplam etkinlik sayısını getirir. IRepository'deki CountAsync'tan farklı olarak siteId bazlı sayım yapar.
        Task<int> GetTotalCountAsync(int siteId);
    }
}
