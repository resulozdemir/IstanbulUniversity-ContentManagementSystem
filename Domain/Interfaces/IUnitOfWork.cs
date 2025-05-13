using System;
using System.Threading.Tasks;

namespace new_cms.Domain.Interfaces
{
    /// Unit of Work desenini temsil eden arayüz.
    /// Veritabanı işlemlerini tek bir transaction altında toplar.
    public interface IUnitOfWork : IDisposable
    {
        /// Belirtilen entity tipi için bir repository örneği döndürür.
        /// <typeparam name="TEntity">Repository'si alınacak entity tipi.
        /// <returns>Entity tipi için IRepository örneği.
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;

        /// Yapılan tüm değişiklikleri veritabanına kaydeder.
        /// <returns>Etkilenen satır sayısı.
        Task<int> CompleteAsync();
    }
}

