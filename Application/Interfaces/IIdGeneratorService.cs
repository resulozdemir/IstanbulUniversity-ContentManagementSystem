using System.Threading.Tasks;

namespace new_cms.Application.Interfaces
{
    /// Veritabanı tablolarında manuel ID üretimi için ortak servis arayüzü
    public interface IIdGeneratorService
    {
        /// Belirtilen entity türü için bir sonraki geçerli ID'yi üretir
        Task<int> GenerateNextIdAsync<TEntity>() where TEntity : class;
    }
} 