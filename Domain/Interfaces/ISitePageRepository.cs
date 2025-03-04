using System.Collections.Generic;
using System.Threading.Tasks;
using new_cms.Domain.Entities;
using new_cms.Application.DTOs;

namespace new_cms.Domain.Interfaces
{
    public interface ISitePageRepository : IRepository<TAppSitepage> //sayfaların yönetimi için repository
    {
        // Belirtilen site ID'sine ait tüm sayfaları listeler. 
        // Site içerik yönetimi ve sayfa listeleme ekranı için gerekli.
        Task<IEnumerable<SitePageDto>> GetPagesBySiteIdAsync(int siteId);

        // Belirtilen sayfa ID'sine ait detaylı bilgileri döndürür.
        // Sayfa düzenleme ve önizleme ekranları için gerekli.
        Task<SitePageDto> GetPageByIdAsync(int id);

        // Belirtilen site ID'sine ait varsayılan (default) sayfayı döndürür.
        // Site ana sayfa yönlendirmesi ve varsayılan içerik gösterimi için gerekli.
        Task<SitePageDto> GetDefaultPageBySiteIdAsync(int siteId);

        // Sayfanın menülerde veya diğer içeriklerde kullanılıp kullanılmadığını kontrol eder.
        // Sayfa silme işlemi öncesi bağımlılık kontrolü için gerekli.
        Task<bool> IsPageInUseAsync(int id);

        // Belirtilen site ID'sine ait sayfa hiyerarşisini ağaç yapısında döndürür.
        // Site haritası, menü yapısı ve içerik organizasyonu için gerekli.
        Task<IEnumerable<PageTreeDto>> GetPageTreeBySiteIdAsync(int siteId);

        // Sayfa yönlendirme (routing) adresinin site içinde benzersiz olup olmadığını kontrol eder.
        // Sayfa ekleme ve düzenleme işlemlerinde URL çakışmalarını önlemek için gerekli.
        Task<bool> IsRoutingUniqueAsync(int siteId, string routing, int? excludePageId = null);

        // Belirtilen site ID ve yönlendirme adresine sahip sayfayı döndürür.
        // URL tabanlı sayfa yönlendirmesi ve içerik gösterimi için gerekli.
        Task<SitePageDto> GetPageByRoutingAsync(int siteId, string routing);
    }
} 