using System;
using System.Collections;
using System.Threading.Tasks;
using new_cms.Domain.Interfaces;
using new_cms.Infrastructure.Persistence;
using new_cms.Domain.Entities;
using new_cms.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace new_cms.Infrastructure.Persistence
{
    /// Unit of Work deseninin somut implementasyonu.
    /// DbContext'i yönetir ve repository örneklerini sağlar
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UCmsContext _context;
        private Hashtable _repositories; // Repository örneklerini saklamak için Hashtable
        private bool disposed = false;


        /// UnitOfWork sınıfının bir örneğini oluşturur.
        /// <param name="context">Kullanılacak DbContext örneği.
        public UnitOfWork(UCmsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _repositories = new Hashtable();
        }


        /// Belirtilen entity tipi için bir repository örneği döndürür.
        /// Eğer örnek daha önce oluşturulmamışsa, yeni bir tane oluşturur ve saklar.
        /// <typeparam name="TEntity">Repository'si alınacak entity tipi.
        /// <returns>Entity tipi için IRepository örneği.
        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(BaseRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);

                if (repositoryInstance != null)
                {
                    _repositories.Add(type, repositoryInstance);
                }
            }

            return (IRepository<TEntity>)(_repositories[type] ?? 
                throw new InvalidOperationException($"Repository oluşturulamadı: {type}"));
        }


        /// Yapılan tüm değişiklikleri veritabanına asenkron olarak kaydeder.
        public async Task<int> CompleteAsync()
        {
            // DbContext üzerinden değişiklikleri kaydet
            return await _context.SaveChangesAsync();
        }


        /// DbContext kaynağını serbest bırakır.
        public void Dispose()
        {   
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// Yönetilen ve yönetilmeyen kaynakları serbest bırakır.
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // DbContext'i dispose et
                    _context.Dispose();
                }
            }
            disposed = true;
        }
    }
}

