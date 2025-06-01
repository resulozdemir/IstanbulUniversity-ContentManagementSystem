using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using new_cms.Application.DTOs.UploadDTOs;
using new_cms.Application.Interfaces;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace new_cms.Application.Services
{
    /// Dosya yükleme ve yönetimi işlemlerini gerçekleştiren servis sınıfı.
    public class UploadService : IUploadService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // İzin verilen dosya türleri
        private readonly string[] _allowedImageTypes = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        private readonly string[] _allowedDocumentTypes = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt" };
        private readonly long _maxFileSize = 10 * 1024 * 1024; // 10MB

        public UploadService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        /// Dosya yükleme işlemini gerçekleştirir
        public async Task<FileUploadResponseDto> UploadFileAsync(FileUploadRequestDto request)
        {
            if (request?.File == null)
                throw new ArgumentNullException(nameof(request), "Yüklenecek dosya boş olamaz.");

            if (request.SiteId <= 0)
                throw new ArgumentException("Geçerli bir Site ID'si gereklidir.", nameof(request.SiteId));

            if (request.UserId <= 0)
                throw new ArgumentException("Geçerli bir User ID'si gereklidir.", nameof(request.UserId));

            try
            {
                // Dosya doğrulamaları
                if (!IsFileTypeAllowed(request.File.FileName))
                    throw new InvalidOperationException($"Dosya türü desteklenmiyor: {Path.GetExtension(request.File.FileName)}");

                if (!IsFileSizeAllowed(request.File.Length))
                    throw new InvalidOperationException($"Dosya boyutu çok büyük. Maksimum: {_maxFileSize / (1024 * 1024)}MB");

                // Dosya yolunu oluştur
                var uploadPath = await CreateUploadPathAsync(request.Category, request.SubFolder);
                var fileName = GenerateUniqueFileName(request.File.FileName);
                var fullPath = Path.Combine(uploadPath, fileName);

                // Klasörü oluştur (yoksa)
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

                // Dosyayı kaydet
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }

                // Veritabanına kayıt ekle
                var uploadFile = new TAppUploadfile
                {
                    Userid = request.UserId,
                    Siteid = request.SiteId,
                    Fileid = Guid.NewGuid().ToString(),
                    Salt = GenerateSalt(),
                    Path = GetRelativePath(fullPath),
                    Type = request.File.ContentType,
                    Filesize = request.File.Length,
                    Filename = request.File.FileName,
                    Createddate = DateTime.UtcNow,
                    Createduser = request.UserId,
                    Isdeleted = false
                };

                var savedFile = await _unitOfWork.Repository<TAppUploadfile>().AddAsync(uploadFile);
                await _unitOfWork.CompleteAsync();

                // Thumbnail oluştur (resim dosyaları için)
                string? thumbnailUrl = null;
                if (request.CreateThumbnail && IsImageFile(request.File.FileName))
                {
                    thumbnailUrl = await CreateThumbnailAsync(fullPath, fileName, request.Category);
                }

                return new FileUploadResponseDto
                {
                    FileId = savedFile.Id,
                    FileName = fileName,
                    OriginalFileName = request.File.FileName,
                    FilePath = GetRelativePath(fullPath),
                    FileUrl = GetFileUrl(GetRelativePath(fullPath)),
                    FileSize = request.File.Length,
                    ContentType = request.File.ContentType,
                    ThumbnailUrl = thumbnailUrl,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new FileUploadResponseDto
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// Birden fazla dosya yükleme işlemini gerçekleştirir
        public async Task<IEnumerable<FileUploadResponseDto>> UploadMultipleFilesAsync(IEnumerable<FileUploadRequestDto> requests)
        {
            var results = new List<FileUploadResponseDto>();

            foreach (var request in requests)
            {
                var result = await UploadFileAsync(request);
                results.Add(result);
            }

            return results;
        }

        /// Belirtilen ID'ye sahip dosyayı getirir
        public async Task<UploadFileDto?> GetFileByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Geçerli bir dosya ID'si gereklidir.", nameof(id));

            try
            {
                var file = await _unitOfWork.Repository<TAppUploadfile>().Query()
                    .FirstOrDefaultAsync(f => f.Id == id && !f.Isdeleted);

                return file == null ? null : _mapper.Map<UploadFileDto>(file);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Dosya getirilirken bir hata oluştu (ID: {id}).", ex);
            }
        }

        /// Belirtilen site ID'sine ait dosyaları listeler
        public async Task<IEnumerable<UploadFileDto>> GetFilesBySiteIdAsync(int siteId)
        {
            if (siteId <= 0)
                throw new ArgumentException("Geçerli bir Site ID'si gereklidir.", nameof(siteId));

            try
            {
                var files = await _unitOfWork.Repository<TAppUploadfile>().Query()
                    .Where(f => f.Siteid == siteId && !f.Isdeleted)
                    .OrderByDescending(f => f.Createddate)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<UploadFileDto>>(files);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Site dosyaları getirilirken bir hata oluştu (Site ID: {siteId}).", ex);
            }
        }

        /// Belirtilen kullanıcı ID'sine ait dosyaları listeler
        public async Task<IEnumerable<UploadFileDto>> GetFilesByUserIdAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Geçerli bir User ID'si gereklidir.", nameof(userId));

            try
            {
                var files = await _unitOfWork.Repository<TAppUploadfile>().Query()
                    .Where(f => f.Userid == userId && !f.Isdeleted)
                    .OrderByDescending(f => f.Createddate)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<UploadFileDto>>(files);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Kullanıcı dosyaları getirilirken bir hata oluştu (User ID: {userId}).", ex);
            }
        }

        /// Sayfalı dosya listesi getirir
        public async Task<(IEnumerable<UploadFileDto> Items, int TotalCount)> GetPagedFilesAsync(
            int pageNumber, int pageSize, int? siteId = null, int? userId = null, 
            string? fileType = null, string? searchTerm = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            try
            {
                var query = _unitOfWork.Repository<TAppUploadfile>().Query().Where(f => !f.Isdeleted);

                if (siteId.HasValue && siteId.Value > 0)
                    query = query.Where(f => f.Siteid == siteId.Value);

                if (userId.HasValue && userId.Value > 0)
                    query = query.Where(f => f.Userid == userId.Value);

                if (!string.IsNullOrWhiteSpace(fileType))
                    query = query.Where(f => f.Type != null && f.Type.Contains(fileType));

                if (!string.IsNullOrWhiteSpace(searchTerm))
                    query = query.Where(f => f.Filename != null && f.Filename.Contains(searchTerm));

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderByDescending(f => f.Createddate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (_mapper.Map<IEnumerable<UploadFileDto>>(items), totalCount);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Dosyalar listelenirken bir hata oluştu.", ex);
            }
        }

        /// Dosyayı siler (soft delete)
        public async Task DeleteFileAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Geçerli bir dosya ID'si gereklidir.", nameof(id));

            try
            {
                var file = await _unitOfWork.Repository<TAppUploadfile>().GetByIdAsync(id);
                if (file == null || file.Isdeleted)
                    throw new KeyNotFoundException($"Silinecek dosya bulunamadı: ID {id}");

                file.Isdeleted = true;
                file.Modifieddate = DateTime.UtcNow;

                await _unitOfWork.Repository<TAppUploadfile>().UpdateAsync(file);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException || ex is ArgumentException) throw;
                throw new InvalidOperationException($"Dosya silinirken bir hata oluştu (ID: {id}).", ex);
            }
        }

        /// Dosyayı fiziksel olarak siler
        public async Task DeleteFilePhysicallyAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Geçerli bir dosya ID'si gereklidir.", nameof(id));

            try
            {
                var file = await _unitOfWork.Repository<TAppUploadfile>().GetByIdAsync(id);
                if (file == null)
                    throw new KeyNotFoundException($"Silinecek dosya bulunamadı: ID {id}");

                // Fiziksel dosyayı sil
                if (!string.IsNullOrEmpty(file.Path))
                {
                    var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, file.Path.TrimStart('/'));
                    if (File.Exists(fullPath))
                        File.Delete(fullPath);
                }

                // Veritabanından sil
                await _unitOfWork.Repository<TAppUploadfile>().DeleteAsync(id);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException || ex is ArgumentException) throw;
                throw new InvalidOperationException($"Dosya fiziksel olarak silinirken bir hata oluştu (ID: {id}).", ex);
            }
        }

        /// Upload app ayarlarını getirir
        public async Task<UploadAppDto?> GetUploadAppByKeyAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Geçerli bir anahtar gereklidir.", nameof(key));

            try
            {
                var uploadApp = await _unitOfWork.Repository<TAppUploadapp>().Query()
                    .FirstOrDefaultAsync(u => u.Key == key && u.Isdeleted == 0);

                return uploadApp == null ? null : _mapper.Map<UploadAppDto>(uploadApp);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Upload app ayarları getirilirken bir hata oluştu (Key: {key}).", ex);
            }
        }

        /// Upload app ayarlarını oluşturur
        public async Task<UploadAppDto> CreateUploadAppAsync(UploadAppDto uploadAppDto)
        {
            if (uploadAppDto == null)
                throw new ArgumentNullException(nameof(uploadAppDto), "Upload app bilgileri boş olamaz.");

            if (string.IsNullOrWhiteSpace(uploadAppDto.Key))
                throw new ArgumentException("Upload app anahtarı boş olamaz.", nameof(uploadAppDto.Key));

            try
            {
                var uploadApp = _mapper.Map<TAppUploadapp>(uploadAppDto);
                uploadApp.Isdeleted = 0;
                uploadApp.Createddate = DateTime.UtcNow;

                var createdUploadApp = await _unitOfWork.Repository<TAppUploadapp>().AddAsync(uploadApp);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<UploadAppDto>(createdUploadApp);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Upload app oluşturulurken bir hata oluştu.", ex);
            }
        }

        /// Upload app ayarlarını günceller
        public async Task<UploadAppDto> UpdateUploadAppAsync(UploadAppDto uploadAppDto)
        {
            if (uploadAppDto?.Id == null || uploadAppDto.Id <= 0)
                throw new ArgumentNullException(nameof(uploadAppDto), "Güncelleme için geçerli bir Upload App ID'si gereklidir.");

            try
            {
                var existingUploadApp = await _unitOfWork.Repository<TAppUploadapp>().GetByIdAsync(uploadAppDto.Id.Value);
                if (existingUploadApp == null || existingUploadApp.Isdeleted == 1)
                    throw new KeyNotFoundException($"Güncellenecek upload app bulunamadı: ID {uploadAppDto.Id.Value}");

                _mapper.Map(uploadAppDto, existingUploadApp);
                existingUploadApp.Modifieddate = DateTime.UtcNow;

                await _unitOfWork.Repository<TAppUploadapp>().UpdateAsync(existingUploadApp);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<UploadAppDto>(existingUploadApp);
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException || ex is ArgumentException) throw;
                throw new InvalidOperationException($"Upload app güncellenirken bir hata oluştu (ID: {uploadAppDto.Id.Value}).", ex);
            }
        }

        /// Dosya türünün yüklenebilir olup olmadığını kontrol eder
        public bool IsFileTypeAllowed(string fileName, string? allowedTypes = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            
            if (!string.IsNullOrWhiteSpace(allowedTypes))
            {
                var types = allowedTypes.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim().ToLowerInvariant()).ToArray();
                return types.Contains(extension);
            }

            // Varsayılan izin verilen türler
            return _allowedImageTypes.Contains(extension) || _allowedDocumentTypes.Contains(extension);
        }

        /// Dosya boyutunun limiti aşıp aşmadığını kontrol eder
        public bool IsFileSizeAllowed(long fileSize, long? maxSize = null)
        {
            var limit = maxSize ?? _maxFileSize;
            return fileSize <= limit && fileSize > 0;
        }

        #region Private Methods

        /// Benzersiz dosya adı oluşturur
        private string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var fileName = Path.GetFileNameWithoutExtension(originalFileName);
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var guid = Guid.NewGuid().ToString("N")[..8];
            
            return $"{fileName}_{timestamp}_{guid}{extension}";
        }

        /// Salt değeri oluşturur
        private string GenerateSalt()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// Upload yolunu oluşturur
        private async Task<string> CreateUploadPathAsync(string category, string? subFolder)
        {
            var basePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", category);
            
            if (!string.IsNullOrWhiteSpace(subFolder))
                basePath = Path.Combine(basePath, subFolder);

            var datePath = DateTime.UtcNow.ToString("yyyy/MM");
            return Path.Combine(basePath, datePath);
        }

        /// Göreli yol döndürür
        private string GetRelativePath(string fullPath)
        {
            var webRootPath = _webHostEnvironment.WebRootPath;
            return fullPath.Replace(webRootPath, "").Replace("\\", "/");
        }

        /// Dosya URL'sini oluşturur
        private string GetFileUrl(string relativePath)
        {
            return relativePath.StartsWith("/") ? relativePath : "/" + relativePath;
        }

        /// Dosyanın resim dosyası olup olmadığını kontrol eder
        private bool IsImageFile(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return _allowedImageTypes.Contains(extension);
        }

        /// Thumbnail oluşturur (basit implementasyon)
        private async Task<string?> CreateThumbnailAsync(string originalPath, string fileName, string category)
        {
            try
            {
                // TODO: Image processing library kullanarak thumbnail oluşturun
                // Şimdilik orijinal dosya yolunu döndürüyoruz
                await Task.CompletedTask;
                return GetFileUrl(GetRelativePath(originalPath));
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
} 