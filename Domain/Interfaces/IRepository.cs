using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace new_cms.Domain.Interfaces
{
    /// Tüm entity'ler için temel CRUD ve sorgulama işlemlerini tanımlayan generic repository arayüzü.
    public interface IRepository<T> where T : class
    {
        /// Belirtilen ID'ye sahip entity'yi asenkron olarak getirir.
        Task<T?> GetByIdAsync(int id);

        /// Tüm entity'leri asenkron olarak listeler.
        Task<IEnumerable<T>> GetAllAsync();

        /// Belirtilen koşula uyan entity'leri asenkron olarak bulur.
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// Yeni bir entity'yi asenkron olarak ekler.
        Task<T> AddAsync(T entity);

        /// Mevcut bir entity'yi asenkron olarak günceller.
        Task<T> UpdateAsync(T entity);

        /// Belirtilen ID'ye sahip entity'yi veritabanından kalıcı olarak asenkron siler.
        Task DeleteAsync(int id);

        /// Belirtilen ID'ye sahip entity'yi silindi olarak işaretler (soft delete).
        Task SoftDeleteAsync(int id);

        /// Çoklu entity ekleme işlemi yapar.
        Task AddRangeAsync(IEnumerable<T> entities);

        /// Çoklu entity güncelleme işlemi yapar.
        Task UpdateRangeAsync(IEnumerable<T> entities);

        /// Çoklu entity silme işlemi yapar (kalıcı).
        Task DeleteRangeAsync(IEnumerable<T> entities); // ID listesi yerine entity listesi

        /// Çoklu entity silme işlemi yapar (soft delete).
        Task SoftDeleteRangeAsync(IEnumerable<T> entities); // Soft delete için eklendi

        /// Belirtilen ID'ye sahip verinin var olup olmadığını kontrol eder.
        Task<bool> ExistsAsync(int id);

        /// Belirtilen koşula uyan herhangi bir kaydın olup olmadığını kontrol eder.
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        /// Belirtilen koşula uyan kayıt sayısını döndürür.
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

        /// Temel sorgu nesnesini (IQueryable) döndürür. Bu, servis katmanında daha karmaşık sorgular oluşturmak için kullanılabilir.
        /// Önemli: Bu metodu kullanırken sorgu veritabanında çalıştırılana kadar ertelenir (deferred execution).
        IQueryable<T> Query();
    }
} 