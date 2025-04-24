using System.Collections.Generic;
using System.Threading.Tasks;
using new_cms.Application.DTOs.PageDTOs;

namespace new_cms.Application.Interfaces
{
    public interface IPageService
    {
        // Tüm sayfaları listeler
        Task<IEnumerable<PageListDto>> GetAllPagesAsync();
        
        // Belirli bir sayfa detayını getirir
        Task<PageDetailDto?> GetPageByIdAsync(int id);
        
        // Yeni sayfa oluşturur
        Task<PageDto> CreatePageAsync(PageDto pageDto);
        
        // Mevcut sayfayı günceller
        Task<PageDto> UpdatePageAsync(PageDto pageDto);
        
        // Sayfayı soft delete yapar
        Task DeletePageAsync(int id);
        
        // Site ID'ye göre sayfaları listeler
        Task<IEnumerable<PageListDto>> GetPagesBySiteIdAsync(int siteId);
        
        // URL'ye göre sayfa getirir
        Task<PageDetailDto?> GetPageByUrlAsync(string url, int siteId);
        
        // Aktif (yayında) olan sayfaları listeler
        Task<IEnumerable<PageListDto>> GetActivePagesAsync();
        
        // Sayfalı ve filtrelenmiş sayfa listesi döndürür
        Task<(IEnumerable<PageListDto> Items, int TotalCount)> GetPagedPagesAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true);
            
        // Ana sayfaları listeler (üst sayfası olmayan)
        Task<IEnumerable<PageListDto>> GetMainPagesAsync(int siteId);
        
        // Alt sayfaları listeler
        Task<IEnumerable<PageListDto>> GetSubPagesAsync(int parentId);
        
        // Sayfa ağacını oluşturur (hiyerarşik yapı)
        Task<IEnumerable<PageTreeDto>> GetPageTreeAsync(int siteId);
        
        // Menüde görünecek sayfaları listeler
        Task<IEnumerable<PageListDto>> GetMenuPagesAsync(int siteId);
    }
} 