using new_cms.Application.DTOs.PageDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.Application.Interfaces
{
    /// Site sayfaları (TAppSitepage) yönetimi ile ilgili operasyonları tanımlayan arayüz.
    public interface IPageService
    {
        /// Sayfalı ve filtrelenmiş sayfa listesi getirir.
        Task<(IEnumerable<PageListDto> Items, int TotalCount)> GetPagedPagesAsync(
            int pageNumber, 
            int pageSize, 
            int? siteId = null, 
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true);

        /// Belirtilen siteye ait tüm aktif sayfaları listeler.
        Task<IEnumerable<PageListDto>> GetPagesBySiteIdAsync(int siteId);


        /// Belirtilen ID'ye sahip aktif sayfayı detaylarıyla getirir.
        Task<PageDetailDto?> GetPageByIdAsync(int id);

        /// Yeni bir site sayfası oluşturur.
        Task<PageDto> CreatePageAsync(PageDto pageDto);


        /// Mevcut bir site sayfasının temel bilgilerini günceller.
        Task<PageDto> UpdatePageAsync(PageDto pageDto);


        /// Belirtilen ID'ye sahip sayfayı pasif hale getirir (soft delete).
        Task DeletePageAsync(int id);
    }
} 