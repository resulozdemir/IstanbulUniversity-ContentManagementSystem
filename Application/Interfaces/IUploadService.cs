using new_cms.Application.DTOs.UploadDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.Application.Interfaces
{
    /// Dosya yükleme ve yönetimi işlemlerini sağlayan arayüz.
    public interface IUploadService
    {
        /// Dosya yükleme işlemini gerçekleştirir.
        Task<FileUploadResponseDto> UploadFileAsync(FileUploadRequestDto request);

        /// Birden fazla dosya yükleme işlemini gerçekleştirir.
        Task<IEnumerable<FileUploadResponseDto>> UploadMultipleFilesAsync(IEnumerable<FileUploadRequestDto> requests);

        /// Belirtilen ID'ye sahip dosyayı getirir.
        Task<UploadFileDto?> GetFileByIdAsync(int id);

        /// Belirtilen site ID'sine ait dosyaları listeler.
        Task<IEnumerable<UploadFileDto>> GetFilesBySiteIdAsync(int siteId);

        /// Belirtilen kullanıcı ID'sine ait dosyaları listeler.
        Task<IEnumerable<UploadFileDto>> GetFilesByUserIdAsync(int userId);

        /// Sayfalı dosya listesi getirir.
        Task<(IEnumerable<UploadFileDto> Items, int TotalCount)> GetPagedFilesAsync(
            int pageNumber, 
            int pageSize, 
            int? siteId = null, 
            int? userId = null,
            string? fileType = null,
            string? searchTerm = null);

        /// Dosyayı siler (soft delete).
        Task DeleteFileAsync(int id);

        /// Dosyayı fiziksel olarak siler.
        Task DeleteFilePhysicallyAsync(int id);

        /// Upload app ayarlarını getirir.
        Task<UploadAppDto?> GetUploadAppByKeyAsync(string key);

        /// Upload app ayarlarını oluşturur.
        Task<UploadAppDto> CreateUploadAppAsync(UploadAppDto uploadAppDto);

        /// Upload app ayarlarını günceller.
        Task<UploadAppDto> UpdateUploadAppAsync(UploadAppDto uploadAppDto);

        /// Dosya türünün yüklenebilir olup olmadığını kontrol eder.
        bool IsFileTypeAllowed(string fileName, string? allowedTypes = null);

        /// Dosya boyutunun limiti aşıp aşmadığını kontrol eder.
        bool IsFileSizeAllowed(long fileSize, long? maxSize = null);
    }
} 