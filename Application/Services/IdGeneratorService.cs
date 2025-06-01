using Microsoft.EntityFrameworkCore;
using new_cms.Application.Interfaces;
using new_cms.Domain.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace new_cms.Application.Services
{
    /// Veritabanı tablolarında manuel ID üretimi için ortak servis
    public class IdGeneratorService : IIdGeneratorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public IdGeneratorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// Belirtilen entity türü için bir sonraki geçerli ID'yi üretir
        public async Task<int> GenerateNextIdAsync<TEntity>() where TEntity : class
        {
            try
            {
                // Entity'nin Id property'sini reflection ile bul
                var entityType = typeof(TEntity);
                var idProperty = entityType.GetProperty("Id");
                
                if (idProperty == null)
                {
                    throw new InvalidOperationException($"{entityType.Name} entity'sinde 'Id' property'si bulunamadı.");
                }

                // Cache problemlerini önlemek için AsNoTracking() kullan
                var query = _unitOfWork.Repository<TEntity>().Query().AsNoTracking();
                
                // Dynamic olarak MaxAsync çağır
                var maxIdObject = await query
                    .Select(e => EF.Property<int?>(e, "Id"))
                    .MaxAsync();
                
                var maxId = maxIdObject ?? 0;
                return maxId + 1;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"{typeof(TEntity).Name} için ID üretilirken hata oluştu: {ex.Message}", ex);
            }
        }
    }
} 