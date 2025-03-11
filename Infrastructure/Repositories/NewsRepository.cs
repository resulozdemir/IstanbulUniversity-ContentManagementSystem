using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using new_cms.Application.Interfaces;
using new_cms.Domain.Entities;
using new_cms.Infrastructure.Persistence.Repositories;

namespace new_cms.Infrastructure.Repositories
{
    // Haber yönetimi için veritabanı işlemlerini gerçekleştiren repository sınıfı
    public class NewsRepository : BaseRepository<TAppNews>, INewsRepository
    {
        public NewsRepository(UCmsContext context) : base(context)
        {
        }

        // Haber ID ve Site ID'ye göre tek bir haber getir
        public new async Task<TAppNews> GetByIdAsync(int id, int siteId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(n => n.Id == id && n.Siteid == siteId && n.Isdeleted == 0);
        }

        // Site ID'ye göre aktif haberleri getir
        public async Task<IEnumerable<TAppNews>> GetActiveNewsAsync(int siteId)
        {
            return await _dbSet
                .Where(n => n.Siteid == siteId && n.Isdeleted == 0 && n.Ispublish == 1)
                .OrderByDescending(n => n.Ondate)
                .ToListAsync();
        }

        // Site ID'ye göre tum haberleri getir
        public async Task<IEnumerable<TAppNews>> GetBySiteIdAsync(int siteId)
        {
            return await _dbSet
                .Where(n => n.Siteid == siteId && n.Isdeleted == 0)
                .OrderByDescending(n => n.Ondate)
                .ToListAsync();
        }

        // Site ID'ye göre slider haberleri getir
        public async Task<IEnumerable<TAppNews>> GetSliderNewsAsync(int siteId)
        {
            return await _dbSet
                .Where(n => n.Siteid == siteId && n.Isdeleted == 0 && n.Inslider == 1 && n.Ispublish == 1)
                .OrderByDescending(n => n.Ondate)
                .ToListAsync();
        }

        // Site ID'ye göre en son haberleri getir
        public async Task<IEnumerable<TAppNews>> GetLatestNewsAsync(int siteId, int count)
        {
            return await _dbSet
                .Where(n => n.Siteid == siteId && n.Isdeleted == 0 && n.Ispublish == 1)
                .OrderByDescending(n => n.Ondate)
                .Take(count)
                .ToListAsync();
        }

        // Site ID'ye göre toplam haber sayısını getir
        public async Task<int> GetTotalCountAsync(int siteId)
        {
            return await _dbSet
                .CountAsync(n => n.Siteid == siteId && n.Isdeleted == 0 && n.Ispublish == 1);
        }

        // Haber ID ve Site ID'ye göre haber varlığını kontrol et
        public new async Task<bool> ExistsAsync(int id, int siteId)
        {
            return await _dbSet.AnyAsync(n => n.Id == id && n.Siteid == siteId && n.Isdeleted == 0);
        }

        // Haber ID ve Site ID'ye göre silme işlemi (Soft Delete)
        public new async Task DeleteAsync(int id, int siteId)
        {
            var news = await GetByIdAsync(id, siteId);
            if (news != null)
            {
                news.Isdeleted = 1;
                await UpdateAsync(news);
            }
        }
    }
} 