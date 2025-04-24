using System.Collections.Generic;
using System.Threading.Tasks;
using new_cms.Application.DTOs.NewsDTOs;

namespace new_cms.Application.Interfaces
{
    public interface INewsService
    {
        // Tüm haberleri listeler
        Task<IEnumerable<NewsListDto>> GetAllNewsAsync();
        
        // Belirli bir haber detayını getirir
        Task<NewsDetailDto?> GetNewsByIdAsync(int id);
        
        // Yeni haber oluşturur
        Task<NewsDto> CreateNewsAsync(NewsDto newsDto);
        
        // Mevcut haberi günceller
        Task<NewsDto> UpdateNewsAsync(NewsDto newsDto);
        
        // Haberi soft delete yapar
        Task DeleteNewsAsync(int id);
        
        // Site ID'ye göre haberleri listeler
        Task<IEnumerable<NewsListDto>> GetNewsBySiteIdAsync(int siteId);
        
        // Kategori ID'ye göre haberleri listeler
        Task<IEnumerable<NewsListDto>> GetNewsByCategoryIdAsync(int categoryId);
        
        // URL'ye göre haber getirir
        Task<NewsDetailDto?> GetNewsByUrlAsync(string url, int siteId);
        
        // Aktif (yayında) olan haberleri listeler
        Task<IEnumerable<NewsListDto>> GetActiveNewsAsync();
        
        // Sayfalı ve filtrelenmiş haber listesi döndürür
        Task<(IEnumerable<NewsListDto> Items, int TotalCount)> GetPagedNewsAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true);
            
        // Öne çıkan haberleri listeler
        Task<IEnumerable<NewsListDto>> GetFeaturedNewsAsync(int siteId);
        
        // En çok okunan haberleri listeler
        Task<IEnumerable<NewsListDto>> GetMostViewedNewsAsync(int siteId, int count = 5);
        
        // Belirli bir tarih aralığındaki haberleri listeler
        Task<IEnumerable<NewsListDto>> GetNewsByDateRangeAsync(int siteId, System.DateTime startDate, System.DateTime endDate);
        
        // Etiket adına göre haberleri listeler
        Task<IEnumerable<NewsListDto>> GetNewsByTagAsync(string tag, int siteId);
    }
} 