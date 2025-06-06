using new_cms.Application.DTOs.NoticeDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.Application.Interfaces
{
    /// Duyuru (Notice) içerik türünün yönetimini sağlayan arayüz.
    public interface INoticeService
    {
        /// Sayfalı ve filtrelenmiş duyuru listesi getirir.
        Task<(IEnumerable<NoticeListDto> Items, int TotalCount)> GetPagedNoticesAsync(
            int pageNumber, 
            int pageSize, 
            int? siteId = null, 
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true);

        /// Belirtilen ID'ye sahip duyuruyu getirir.
        Task<NoticeDto?> GetNoticeByIdAsync(int id);

        /// Yeni bir duyuru oluşturur.
        Task<NoticeDto> CreateNoticeAsync(NoticeDto noticeDto);

        /// Mevcut bir duyuruyu günceller.
        Task<NoticeDto> UpdateNoticeAsync(NoticeDto noticeDto);
        
        /// Belirtilen ID'ye sahip duyuruyu siler (soft delete).
        Task DeleteNoticeAsync(int id);
    }
} 