using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
        }

        /// Belirtilen ID'ye sahip entity'yi asenkron olarak getirir.
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// Tüm entity'leri asenkron olarak listeler.
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// Belirtilen koşula uyan entity'leri asenkron olarak bulur.
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// Yeni bir entity'yi asenkron olarak ekler.
        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// Mevcut bir entity'yi asenkron olarak günceller.
        public virtual async Task<T> UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        /// Belirtilen ID'ye sahip entity'yi veritabanından kalıcı olarak asenkron siler.
        public virtual async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        /// Belirtilen ID'ye sahip entity'yi silindi olarak işaretler (soft delete).
        /// 'IsDeleted' veya 'Isdeleted' property'sini dinamik olarak bulur ve günceller.
        public virtual async Task SoftDeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                SetSoftDeleteProperty(entity, true);
                await _context.SaveChangesAsync();
            }
        }

        /// Çoklu entity ekleme işlemi yapar.
        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        /// Çoklu entity güncelleme işlemi yapar.
        public virtual async Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }

        /// Çoklu entity silme işlemi yapar (kalıcı).
        public virtual async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        /// Çoklu entity silme işlemi yapar (soft delete).
        public virtual async Task SoftDeleteRangeAsync(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                SetSoftDeleteProperty(entity, true);
            }
            await _context.SaveChangesAsync();
        }

        /// Belirtilen ID'ye sahip verinin var olup olmadığını kontrol eder.
        public virtual async Task<bool> ExistsAsync(int id)
        {
            // FindAsync null dönerse kayıt yoktur.
            return await _dbSet.FindAsync(id) != null;
        }

        /// Belirtilen koşula uyan herhangi bir kaydın olup olmadığını kontrol eder.
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        /// Belirtilen koşula uyan kayıt sayısını döndürür.
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(predicate);
        }

        /// Temel sorgu nesnesini (IQueryable) döndürür.
        public virtual IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }

        // --- Helper Metodlar ---

        /// Entity üzerinde 'IsDeleted' veya 'Isdeleted' property'sini bulup değerini ayarlar.
        protected virtual void SetSoftDeleteProperty(T entity, bool value)
        {
            var propertyInfo = GetSoftDeletePropertyInfo();
            if (propertyInfo != null)
            {
                // Property tipine göre değeri ayarla (bool veya int olabilir)
                if (propertyInfo.PropertyType == typeof(bool))
                {
                    propertyInfo.SetValue(entity, value);
                }
                else if (propertyInfo.PropertyType == typeof(int))
                {
                    propertyInfo.SetValue(entity, value ? 1 : 0);
                }
                // Diğer tipler için gerekirse genişletilebilir.
            }
        }

        /// Cache'lenmiş veya dinamik olarak soft delete property bilgisini alır.
        private PropertyInfo? _softDeletePropertyInfo; // Basit caching
        private bool _softDeletePropertyChecked = false;

        protected virtual PropertyInfo? GetSoftDeletePropertyInfo()
        {
            if (!_softDeletePropertyChecked)
            {
                _softDeletePropertyInfo = typeof(T).GetProperty("IsDeleted", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                _softDeletePropertyChecked = true;
            }
            return _softDeletePropertyInfo;
        }
    }
} 