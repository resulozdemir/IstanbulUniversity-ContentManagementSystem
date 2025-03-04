using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;

namespace new_cms.Infrastructure.Persistence.Repositories
{

    /// Tüm repository'ler için temel CRUD ve yaygın veritabanı işlemlerini sağlayan temel sınıf.
    /// Entity Framework Core kullanarak veritabanı işlemlerini gerçekleştirir.
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly UCmsContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(UCmsContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // ID'ye göre tekil kayıt getirme
        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        // Tüm kayıtları listeleme
        public virtual async Task<IEnumerable<T>> GetAllAsync()  //IEnumerable sadece ileriye donuk listeleme yapmak icin kullanilir.
        {
            return await _dbSet.ToListAsync();
        }

        // Belirli bir koşula göre kayıtları filtreleme
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        // Yeni kayıt ekleme
        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // Mevcut kaydı güncelleme
        public virtual async Task<T> UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified; //entity'nin durumunu modified yapıyoruz.
            await _context.SaveChangesAsync();
            return entity;
        }

        // Kaydı kalıcı olarak silme
        public virtual async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        // Kaydı silindi olarak işaretleme (soft delete)
        public virtual async Task SoftDeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                // IsDeleted özelliğini dinamik olarak kontrol edip ayarlama
                var property = typeof(T).GetProperty("IsDeleted");
                if (property != null)
                {
                    property.SetValue(entity, true);
                    await _context.SaveChangesAsync();
                }
            }
        }

        // Sayfalanmış veri getirme
        //tek bir metotda birden fazla deger dondurmek icin tuple (coklu geri donus) kullanilir. Orn : Task<(IEnumerable<T> Items, int TotalCount)>
        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
        {
            var totalCount = await _dbSet.CountAsync();
            var items = await _dbSet.Skip((pageNumber - 1) * pageSize)  // 2. Sayfa: sonraki 10 kayıt (11-20) 'lk 10 kaydi geç
                                  .Take(pageSize)                       // (2-1) * 10 = 10 kayıt atla
                                  .ToListAsync();                       // 10 kayıt al

            return (items, totalCount);
        }

        // Filtrelenmiş, sıralanmış ve ilişkili verilerle birlikte kayıtları getirme orn:actıve ve deleted olmayanlar
        public virtual async Task<IEnumerable<T>> GetFilteredAsync(
            Expression<Func<T, bool>> filter = null,                  //filtreleme icin
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, //siralama icin
            string includeProperties = "")                            //iliskili veriler tablolar icin (pages ve components)
        {
            IQueryable<T> query = _dbSet; //IQueryable : Veritabanı sprgusu olusturmak ıcın kullanılan ınterface, sorgu veritabanına girmeden once olusturulur ve optimize edilir

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        // Çoklu kayıt ekleme
        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        // Çoklu kayıt güncelleme
        public virtual async Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }

        // Çoklu kayıt silme
        public virtual async Task DeleteRangeAsync(IEnumerable<int> ids)
        {
            var entities = await _dbSet.Where(e => ids.Contains((int)e.GetType().GetProperty("Id").GetValue(e))).ToListAsync();
            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        // ID'ye göre kayıt varlığı kontrolü
        public virtual async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.FindAsync(id) != null;
        }

        // Belirli bir koşula göre kayıt varlığı kontrolü
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        // Kayıt sayısı hesaplama
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
                return await _dbSet.CountAsync();
            return await _dbSet.CountAsync(predicate);
        }
    }
} 