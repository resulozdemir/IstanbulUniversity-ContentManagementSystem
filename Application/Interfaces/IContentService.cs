using System.Collections.Generic;
using System.Threading.Tasks;
using new_cms.Application.DTOs.ContentDTOs;

namespace new_cms.Application.Interfaces
{
    public interface IContentService
    {
        // İçerik sayfalarını listeler
        Task<IEnumerable<ContentPageListDto>> GetAllContentPagesAsync();
        
        // Belirli bir içerik sayfası detayını getirir
        Task<ContentPageDetailDto?> GetContentPageByIdAsync(int id);
        
        // Yeni içerik sayfası oluşturur
        Task<ContentPageDto> CreateContentPageAsync(ContentPageDto contentPageDto);
        
        // Mevcut içerik sayfasını günceller
        Task<ContentPageDto> UpdateContentPageAsync(ContentPageDto contentPageDto);
        
        // İçerik sayfasını soft delete yapar
        Task DeleteContentPageAsync(int id);
        
        // İçerik gruplarını listeler
        Task<IEnumerable<ContentGroupListDto>> GetAllContentGroupsAsync();
        
        // Belirli bir içerik grubu detayını getirir
        Task<ContentGroupDetailDto?> GetContentGroupByIdAsync(int id);
        
        // Yeni içerik grubu oluşturur
        Task<ContentGroupDto> CreateContentGroupAsync(ContentGroupDto contentGroupDto);
        
        // Mevcut içerik grubunu günceller
        Task<ContentGroupDto> UpdateContentGroupAsync(ContentGroupDto contentGroupDto);
        
        // İçerik grubunu soft delete yapar
        Task DeleteContentGroupAsync(int id);
        
        // Sayfalı ve filtrelenmiş içerik sayfası listesi döndürür
        Task<(IEnumerable<ContentPageListDto> Items, int TotalCount)> GetPagedContentPagesAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true);
            
        // Sayfalı ve filtrelenmiş içerik grubu listesi döndürür
        Task<(IEnumerable<ContentGroupListDto> Items, int TotalCount)> GetPagedContentGroupsAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true);
            
        // Site ID'ye göre içerik sayfalarını listeler
        Task<IEnumerable<ContentPageListDto>> GetContentPagesBySiteIdAsync(int siteId);
        
        // Grup ID'ye göre içerik sayfalarını listeler
        Task<IEnumerable<ContentPageListDto>> GetContentPagesByGroupIdAsync(int groupId);
        
        // Yayında olan içerik sayfalarını listeler
        Task<IEnumerable<ContentPageListDto>> GetActiveContentPagesAsync();
        
        // Kategorisiz içerik sayfalarını listeler
        Task<IEnumerable<ContentPageListDto>> GetUncategorizedContentPagesAsync();
    }
} 