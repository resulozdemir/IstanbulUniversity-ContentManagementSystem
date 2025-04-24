using new_cms.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.Domain.Interfaces
{
    // İçerik grubu verileri için veritabanı işlemlerini tanımlayan interface, IRepository'den türetilmiştir.
    public interface IContentGroupRepository : IRepository<TAppContentgroup>
    {
        // Belirli bir içerik grubunu ID ve site ID'ye göre getirir. IRepository'deki GetByIdAsync'tan farklı olarak siteId kontrolü yapar.
        Task<TAppContentgroup?> GetByIdAsync(int id, int siteId);

        // Belirli bir siteye ait tüm içerik gruplarını getirir
        Task<IEnumerable<TAppContentgroup>> GetBySiteIdAsync(int siteId);

        // Belirli bir siteye ait aktif içerik gruplarını getirir
        Task<IEnumerable<TAppContentgroup>> GetActiveContentGroupsAsync(int siteId);

        // Belirli bir routing değerine sahip içerik grubunu getirir
        Task<TAppContentgroup?> GetByRoutingAsync(string routing, int siteId);

        // Belirli bir içerik grubunu siler (soft delete). IRepository'deki DeleteAsync'tan farklı olarak siteId kontrolü yapar.
        Task DeleteAsync(int id, int siteId);

        // Belirli bir içerik grubunun varlığını kontrol eder. IRepository'deki ExistsAsync'tan farklı olarak siteId kontrolü yapar.
        Task<bool> ExistsAsync(int id, int siteId);

        // Belirli bir siteye ait toplam içerik grubu sayısını getirir. IRepository'deki CountAsync'tan farklı olarak siteId bazlı sayım yapar.
        Task<int> GetTotalCountAsync(int siteId);
    }
}
