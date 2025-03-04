using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace new_cms.Domain.Interfaces
{
    public interface IRepository<T> where T : class  //Tüm repositoryler için genel CRUD yapısını.
    {
        // Belirtilen ID'ye sahip entity'yi getirir. Temel CRUD operasyonu için gerekli.
        Task<T> GetByIdAsync(int id);

        // Tüm entity'leri listeler. Genel listeleme işlemleri için kullanılır.
        Task<IEnumerable<T>> GetAllAsync();

        // Belirtilen koşula uyan entity'leri bulur. 
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        // Yeni bir entity ekler. 
        Task<T> AddAsync(T entity);

        // Mevcut bir entity'yi günceller. 
        Task<T> UpdateAsync(T entity);

        // Entity'yi veritabanından kalıcı olarak siler. 
        Task DeleteAsync(int id);

        // Entity'yi silindi olarak işaretler (soft delete). 
        Task SoftDeleteAsync(int id);

        // Sayfalama yapılmış veri döndürür. Büyük veri setlerinde performans için gerekli.
        // Örn : Site sayfalarının listelenmesi, sayfa şeklinde haberlerin listelenmesi
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);

        // Filtrelenmiş, sıralanmış ve ilişkili verileri içeren sonuçlar döndürür. Karmaşık sorgular için.
        Task<IEnumerable<T>> GetFilteredAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");

        // Çoklu entity ekleme işlemi yapar. Toplu veri ekleme senaryoları için performans optimizasyonu.
        // Örn : Site şablonundan yeni site oluşturma (tüm içerikleriyle)
        Task AddRangeAsync(IEnumerable<T> entities);

        // Çoklu entity güncelleme işlemi yapar. Toplu güncelleme senaryoları için performans optimizasyonu.
        Task UpdateRangeAsync(IEnumerable<T> entities);

        // Çoklu entity silme işlemi yapar. Toplu silme senaryoları için performans optimizasyonu.
        Task DeleteRangeAsync(IEnumerable<int> ids);

        // Belirtilen ID'ye sahip verinin var olup olmadığını kontrol eder. 
        // Örn : Site sayfasının varlığını kontrol etmek için
        Task<bool> ExistsAsync(int id);

        // Belirtilen koşula uyan herhangi bir kaydın olup olmadığını kontrol eder. Birden fazla koşul da olabilir.
        // Örn : isActive = true olan ve Blog tipi şu olan siteleri döndürmek için
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        // Belirtilen koşula uyan kayıt sayısını döndürür. 
        // Örn : Site sayfasının sayısını döndürmek için
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
    }
} 