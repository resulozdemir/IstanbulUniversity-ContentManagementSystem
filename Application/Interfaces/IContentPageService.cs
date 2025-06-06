using new_cms.Application.DTOs.ContentPageDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.Application.Interfaces
{
    /// Content sayfaları (TAppContentpage) yönetimi ile ilgili operasyonları tanımlayan arayüz
    public interface IContentPageService
    {
        /// Sayfalı ve filtrelenmiş content sayfası listesi getirir
        Task<(IEnumerable<ContentPageListDto> Items, int TotalCount)> GetPagedContentPagesAsync(
            int pageNumber, 
            int pageSize, 
            int? groupId = null,
            int? siteId = null, 
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true);

        /// Belirtilen ID'ye sahip content sayfasını getirir
        Task<ContentPageDto?> GetContentPageByIdAsync(int id);

        /// Yeni bir content sayfası oluşturur
        Task<ContentPageDto> CreateContentPageAsync(ContentPageDto contentPageDto);

        /// Mevcut content sayfasını günceller
        Task<ContentPageDto> UpdateContentPageAsync(ContentPageDto contentPageDto);

        /// Content sayfasını pasif hale getirir (soft delete)
        Task DeleteContentPageAsync(int id);

        /// Belirtilen group'a ait content sayfalarını OrderBy alanına göre sıralı olarak getirir
        Task<IEnumerable<ContentPageListDto>> GetContentPagesByGroupIdAsync(int groupId);

        /// Sistemdeki tüm aktif content sayfalarını getirir
        Task<IEnumerable<ContentPageListDto>> GetActiveContentPagesAsync();

        /// Belirtilen site'ye ait content sayfalarını getirir
        Task<IEnumerable<ContentPageListDto>> GetContentPagesBySiteIdAsync(int siteId);
    }
} 