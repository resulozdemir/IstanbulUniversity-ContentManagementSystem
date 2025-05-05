using new_cms.Application.DTOs.NewsDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.Application.Interfaces
{
    /// Haber (News) içerik türünün yönetimini sağlayan arayüz.
    public interface INewsService
    {
        /// Sayfalı ve filtrelenmiş haber listesi getirir.
        Task<(IEnumerable<NewsListDto> Items, int TotalCount)> GetPagedNewsAsync(
            int pageNumber, 
            int pageSize, 
            int? siteId = null, 
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true);
            
        /// Belirtilen ID'ye sahip haberi getirir.
        Task<NewsDto?> GetNewsByIdAsync(int id);

        /// Yeni bir haber oluşturur.
        Task<NewsDto> CreateNewsAsync(NewsDto newsDto);

        /// Mevcut bir haberi günceller.
        Task<NewsDto> UpdateNewsAsync(NewsDto newsDto);

        /// Belirtilen ID'ye sahip haberi siler (soft delete).
        Task DeleteNewsAsync(int id);
    }
} 