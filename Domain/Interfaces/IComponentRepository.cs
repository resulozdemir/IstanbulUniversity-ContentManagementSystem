using System.Collections.Generic;
using System.Threading.Tasks;
using new_cms.Domain.Entities;
using new_cms.Application.DTOs;

namespace new_cms.Domain.Interfaces
{
    public interface IComponentRepository : IRepository<TAppComponent> //site içerisinde kullanılacak bileşenleri repositorysi (bileşen listeleme, filtreleme)
    {
        // Tüm bileşenleri entity formatında listeler. Bileşen yönetim sayfası için gerekli.
        Task<IEnumerable<TAppComponent>> GetAllComponentsAsync();

        // Belirtilen bileşen ID'sine ait detayları döndürür. Bileşen düzenleme sayfası için gerekli.
        Task<TAppComponent> GetComponentByIdAsync(int id);

        // Bileşenin herhangi bir temada kullanılıp kullanılmadığını kontrol eder. Bileşen silme işlemi öncesi kontrol için.
        Task<bool> IsComponentInUseAsync(int id);

        // Belirtilen tipteki bileşenleri listeler. Bileşen filtreleme için gerekli.
        Task<IEnumerable<TAppComponent>> GetComponentsByTypeAsync(string type);

        // Belirtilen site ID'sine ait bileşen verilerini listeler. Site içerik yönetimi için gerekli.
        Task<IEnumerable<TAppSitecomponentdata>> GetComponentDataBySiteIdAsync(int siteId);

        // Belirtilen bileşen verisinin detaylarını döndürür. Bileşen veri düzenleme sayfası için gerekli.
        Task<TAppSitecomponentdata> GetComponentDataByIdAsync(int dataId);
    }
} 