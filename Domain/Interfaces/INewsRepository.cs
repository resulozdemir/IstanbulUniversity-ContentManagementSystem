using new_cms.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.Domain.Interfaces
{
    // Haber verileri için veritabanı işlemlerini tanımlayan interface, IRepository'den türetilmiştir.
    public interface INewsRepository : IRepository<TAppNews>
    {
        // Belirli bir haberi ID ve site ID'ye göre getirir. IRepository'deki GetByIdAsync'tan farklı olarak siteId kontrolü yapar.
        Task<TAppNews?> GetByIdAsync(int id, int siteId);

        // Belirli bir siteye ait tüm haberleri getirir
        Task<IEnumerable<TAppNews>> GetBySiteIdAsync(int siteId);

        // Belirli bir siteye ait aktif haberleri getirir
        Task<IEnumerable<TAppNews>> GetActiveNewsAsync(int siteId);

        // Belirli bir siteye ait slider'da gösterilecek haberleri getirir
        Task<IEnumerable<TAppNews>> GetSliderNewsAsync(int siteId);

        // Belirli bir haberi siler (soft delete). IRepository'deki DeleteAsync'tan farklı olarak siteId kontrolü yapar.
        // Soft delete işlemi genellikle IRepository.SoftDeleteAsync ile yapılır, ancak burada siteId bazlı ek kontrol gerekebilir.
        // Eğer sadece ID ile soft delete yeterliyse IRepository.SoftDeleteAsync kullanılabilir.
        Task DeleteAsync(int id, int siteId); 
        
        // Belirli bir siteye ait en son haberleri belirtilen sayıda getirir
        Task<IEnumerable<TAppNews>> GetLatestNewsAsync(int siteId, int count);

        // Belirli bir haberin varlığını kontrol eder. IRepository'deki ExistsAsync'tan farklı olarak siteId kontrolü yapar.
        Task<bool> ExistsAsync(int id, int siteId);

        // Belirli bir siteye ait toplam haber sayısını getirir. IRepository'deki CountAsync'tan farklı olarak siteId bazlı sayım yapar.
        Task<int> GetTotalCountAsync(int siteId);
    }
}