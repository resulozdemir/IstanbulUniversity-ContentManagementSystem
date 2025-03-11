using new_cms.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.Application.Interfaces
{
    public interface IContentGroupRepository  // İçerik grubu verileri için veritabanı işlemlerini tanımlayan interface
    {
        // Belirli bir içerik grubunu ID ve site ID'ye göre getirir
        Task<TAppContentgroup> GetByIdAsync(int id, int siteId);

        // Tüm içerik gruplarını getirir
        Task<IEnumerable<TAppContentgroup>> GetAllAsync();

        // Belirli bir siteye ait tüm içerik gruplarını getirir
        Task<IEnumerable<TAppContentgroup>> GetBySiteIdAsync(int siteId);

        // Belirli bir siteye ait aktif içerik gruplarını getirir
        Task<IEnumerable<TAppContentgroup>> GetActiveContentGroupsAsync(int siteId);

        // Belirli bir routing değerine sahip içerik grubunu getirir
        Task<TAppContentgroup> GetByRoutingAsync(string routing, int siteId);

        // Yeni bir içerik grubu ekler
        Task<TAppContentgroup> AddAsync(TAppContentgroup contentGroup);

        // Mevcut bir içerik grubunu günceller
        Task<TAppContentgroup> UpdateAsync(TAppContentgroup contentGroup);

        // Belirli bir içerik grubunu siler (soft delete)
        Task DeleteAsync(int id, int siteId);

        // Belirli bir içerik grubunun varlığını kontrol eder
        Task<bool> ExistsAsync(int id, int siteId);

        // Belirli bir siteye ait toplam içerik grubu sayısını getirir
        Task<int> GetTotalCountAsync(int siteId);
    }
}
