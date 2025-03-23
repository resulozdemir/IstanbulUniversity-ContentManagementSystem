using new_cms.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.Domain.Interfaces
{
    public interface INewsRepository  // Haber verileri için veritabanı işlemlerini tanımlayan interface
    {
        // Belirli bir haberi ID ve site ID'ye göre getirir
        Task<TAppNews> GetByIdAsync(int id, int siteId);

        // Tüm haberleri getirir
        Task<IEnumerable<TAppNews>> GetAllAsync();

        // Belirli bir siteye ait tüm haberleri getirir
        Task<IEnumerable<TAppNews>> GetBySiteIdAsync(int siteId);

        // Belirli bir siteye ait aktif haberleri getirir
        Task<IEnumerable<TAppNews>> GetActiveNewsAsync(int siteId);

        // Belirli bir siteye ait slider'da gösterilecek haberleri getirir
        Task<IEnumerable<TAppNews>> GetSliderNewsAsync(int siteId);

        // Yeni bir haber ekler
        Task<TAppNews> AddAsync(TAppNews news);

        // Mevcut bir haberi günceller
        Task<TAppNews> UpdateAsync(TAppNews news);

        // Belirli bir haberi siler (soft delete)
        Task DeleteAsync(int id, int siteId);
        
        // Belirli bir siteye ait en son haberleri belirtilen sayıda getirir
        Task<IEnumerable<TAppNews>> GetLatestNewsAsync(int siteId, int count);

        // Belirli bir haberin varlığını kontrol eder
        Task<bool> ExistsAsync(int id, int siteId);

        // Belirli bir siteye ait toplam haber sayısını getirir
        Task<int> GetTotalCountAsync(int siteId);
    }
}