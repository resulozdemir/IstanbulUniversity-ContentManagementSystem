using new_cms.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.Application.Interfaces
{
    public interface IEventRepository  // Etkinlik verileri için veritabanı işlemlerini tanımlayan interface
    {
        // Belirli bir etkinliği ID ve site ID'ye göre getirir
        Task<TAppEvent> GetByIdAsync(int id, int siteId);

        // Tüm etkinlikleri getirir
        Task<IEnumerable<TAppEvent>> GetAllAsync();

        // Belirli bir siteye ait tüm etkinlikleri getirir
        Task<IEnumerable<TAppEvent>> GetBySiteIdAsync(int siteId);

        // Belirli bir siteye ait aktif etkinlikleri getirir
        Task<IEnumerable<TAppEvent>> GetActiveEventsAsync(int siteId);

        // Yeni bir etkinlik ekler
        Task<TAppEvent> AddAsync(TAppEvent events);

        // Mevcut bir etkinliği günceller
        Task<TAppEvent> UpdateAsync(TAppEvent events);

        // Belirli bir etkinliği siler (soft delete)
        Task DeleteAsync(int id, int siteId);
        
        // Belirli bir siteye ait en son etkinlikleri belirtilen sayıda getirir
        Task<IEnumerable<TAppEvent>> GetLatestEventsAsync(int siteId, int count);

        // Belirli bir etkinliğin varlığını kontrol eder
        Task<bool> ExistsAsync(int id, int siteId);

        // Belirli bir siteye ait toplam etkinlik sayısını getirir
        Task<int> GetTotalCountAsync(int siteId);
    }
}
