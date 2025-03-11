using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using new_cms.Application.Interfaces;
using new_cms.Domain.Entities;
using new_cms.Infrastructure.Persistence.Repositories;

namespace new_cms.Infrastructure.Repositories
{
    // İçerik grubu yönetimi için veritabanı işlemlerini gerçekleştiren repository sınıfı haberler, sayfalar, blog yazıları vb.
    public class ContentGroupRepository : BaseRepository<TAppContentgroup>, IContentGroupRepository
    {
        public ContentGroupRepository(UCmsContext context) : base(context)
        {
        }

        // İçerik grubu ID ve Site ID'ye göre tek bir içerik grubu getir
        public new async Task<TAppContentgroup> GetByIdAsync(int id, int siteId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Id == id && c.Siteid == siteId && c.Isdeleted == 0);
        }

        // Site ID'ye göre aktif içerik gruplarını getir
        public async Task<IEnumerable<TAppContentgroup>> GetActiveContentGroupsAsync(int siteId)
        {
            return await _dbSet
                .Where(c => c.Siteid == siteId && c.Isdeleted == 0 && c.Isactive == 1)
                .OrderBy(c => c.Order)
                .ToListAsync();
        }

        // Site ID'ye göre tüm içerik gruplarını getir
        public async Task<IEnumerable<TAppContentgroup>> GetBySiteIdAsync(int siteId)
        {
            return await _dbSet
                .Where(c => c.Siteid == siteId && c.Isdeleted == 0)
                .OrderBy(c => c.Order)
                .ToListAsync();
        }

        // Routing ve Site ID'ye göre içerik grubunu getir /icerik/spor-haberleri, /icerik/teknoloji
        public async Task<TAppContentgroup> GetByRoutingAsync(string routing, int siteId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Routing == routing && c.Siteid == siteId && c.Isdeleted == 0 && c.Isactive == 1);
        }

        // Site ID'ye göre toplam içerik grubu sayısını getir
        public async Task<int> GetTotalCountAsync(int siteId)
        {
            return await _dbSet
                .CountAsync(c => c.Siteid == siteId && c.Isdeleted == 0);
        }

        // İçerik grubu ID ve Site ID'ye göre içerik grubu varlığını kontrol et
        public new async Task<bool> ExistsAsync(int id, int siteId)
        {
            return await _dbSet.AnyAsync(c => c.Id == id && c.Siteid == siteId && c.Isdeleted == 0);
        }

        // İçerik grubu ID ve Site ID'ye göre silme işlemi (Soft Delete)
        public new async Task DeleteAsync(int id, int siteId)
        {
            var contentGroup = await GetByIdAsync(id, siteId);
            if (contentGroup != null)
            {
                contentGroup.Isdeleted = 1;
                await UpdateAsync(contentGroup);
            }
        }
    }
}
