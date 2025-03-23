using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;

namespace new_cms.Infrastructure.Persistence.Repositories
{
    // Etkinlik yönetimi için veritabanı işlemlerini gerçekleştiren repository sınıfı
    public class EventRepository : BaseRepository<TAppEvent>, IEventRepository
    {
        public EventRepository(UCmsContext context) : base(context)
        {
        }

        // Etkinlik ID ve Site ID'ye göre tek bir etkinlik getir
        public async Task<TAppEvent> GetByIdAsync(int id, int siteId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(e => e.Id == id && e.Siteid == siteId && e.Isdeleted == 0);
        }

        // Site ID'ye göre aktif etkinlikleri getir
        public async Task<IEnumerable<TAppEvent>> GetActiveEventsAsync(int siteId)
        {
            return await _dbSet
                .Where(e => e.Siteid == siteId && e.Isdeleted == 0 && e.Ispublish == 1)
                .OrderByDescending(e => e.Ondate)
                .ToListAsync();
        }

        // Site ID'ye göre tüm etkinlikleri getir
        public async Task<IEnumerable<TAppEvent>> GetBySiteIdAsync(int siteId)
        {
            return await _dbSet
                .Where(e => e.Siteid == siteId && e.Isdeleted == 0)
                .OrderByDescending(e => e.Ondate)
                .ToListAsync();
        }

        // Site ID'ye göre en son etkinlikleri getir
        public async Task<IEnumerable<TAppEvent>> GetLatestEventsAsync(int siteId, int count)
        {
            return await _dbSet
                .Where(e => e.Siteid == siteId && e.Isdeleted == 0 && e.Ispublish == 1)
                .OrderByDescending(e => e.Ondate)
                .Take(count)
                .ToListAsync();
        }

        // Site ID'ye göre toplam etkinlik sayısını getir
        public async Task<int> GetTotalCountAsync(int siteId)
        {
            return await _dbSet
                .CountAsync(e => e.Siteid == siteId && e.Isdeleted == 0 && e.Ispublish == 1);
        }

        // Etkinlik ID ve Site ID'ye göre etkinlik varlığını kontrol et
        public async Task<bool> ExistsAsync(int id, int siteId)
        {
            return await _dbSet.AnyAsync(e => e.Id == id && e.Siteid == siteId && e.Isdeleted == 0);
        }

        // Etkinlik ID ve Site ID'ye göre silme işlemi (Soft Delete)
        public async Task DeleteAsync(int id, int siteId)
        {
            var events = await GetByIdAsync(id, siteId);
            if (events != null)
            {
                events.Isdeleted = 1;
                await UpdateAsync(events);
            }
        }
    }
}
