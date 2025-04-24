using System.Collections.Generic;
using System.Threading.Tasks;
using new_cms.Application.DTOs.ComponentDTOs;

namespace new_cms.Application.Interfaces
{
    public interface IComponentService
    {
        // Tüm bileşenleri listeler
        Task<IEnumerable<ComponentDto>> GetAllComponentsAsync();
        
        // Belirli bir bileşenin detayını getirir
        Task<ComponentDto?> GetComponentByIdAsync(int id);
        
        // Yeni bileşen oluşturur
        Task<ComponentDto> CreateComponentAsync(ComponentDto componentDto);
        
        // Mevcut bileşeni günceller
        Task<ComponentDto> UpdateComponentAsync(ComponentDto componentDto);
        
        // Bileşeni soft delete yapar
        Task DeleteComponentAsync(int id);
        
        // Belirli türdeki bileşenleri listeler
        Task<IEnumerable<ComponentDto>> GetComponentsByTypeAsync(string type);
        
        // Bileşenin kullanımda olup olmadığını kontrol eder
        Task<bool> IsComponentInUseAsync(int id);
        
        // Belirli bir siteye ait bileşen verilerini listeler
        Task<IEnumerable<SiteComponentDataDto>> GetComponentDataBySiteIdAsync(int siteId);
        
        // Belirli bir bileşen verisinin detaylarını getirir
        Task<SiteComponentDataDto?> GetComponentDataByIdAsync(int dataId);
        
        // Siteye yeni bileşen verisi ekler
        Task<SiteComponentDataDto> AddComponentDataToSiteAsync(SiteComponentDataDto dataDto);
        
        // Bileşen verisini günceller
        Task<SiteComponentDataDto> UpdateComponentDataAsync(SiteComponentDataDto dataDto);
        
        // Bileşen verisini kaldırır
        Task DeleteComponentDataAsync(int dataId);
    }
} 