using Microsoft.AspNetCore.Mvc;
using new_cms.Application.DTOs.UploadDTOs;
using new_cms.Application.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace new_cms.WebApi.Controllers
{
    /// Dosya yükleme ve yönetimi API endpoint'leri
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IUploadService _uploadService;

        public UploadController(IUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        /// Tek dosya yükleme işlemi
        [HttpPost]
        public async Task<ActionResult<FileUploadResponseDto>> UploadFile([FromForm] FileUploadRequestDto request)
        {
            try
            {
                var result = await _uploadService.UploadFileAsync(request);
                
                if (!result.Success)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Dosya yüklenirken beklenmedik bir hata oluştu.", details = ex.Message });
            }
        }

        /// Birden fazla dosya yükleme işlemi
        [HttpPost("multiple")]
        public async Task<ActionResult<IEnumerable<FileUploadResponseDto>>> UploadMultipleFiles([FromForm] IEnumerable<FileUploadRequestDto> requests)
        {
            try
            {
                var results = await _uploadService.UploadMultipleFilesAsync(requests);
                return Ok(results);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Dosyalar yüklenirken beklenmedik bir hata oluştu.", details = ex.Message });
            }
        }

        /// Belirtilen ID'ye sahip dosyayı getirir
        [HttpGet("{id}")]
        public async Task<ActionResult<UploadFileDto>> GetFile(int id)
        {
            try
            {
                var file = await _uploadService.GetFileByIdAsync(id);
                
                if (file == null)
                    return NotFound(new { message = $"Dosya bulunamadı: ID {id}" });

                return Ok(file);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Dosya getirilirken bir hata oluştu.", details = ex.Message });
            }
        }

        /// Sayfalı dosya listesi getirir
        [HttpGet]
        public async Task<ActionResult> GetFiles(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? siteId = null,
            [FromQuery] int? userId = null,
            [FromQuery] string? fileType = null,
            [FromQuery] string? searchTerm = null)
        {
            try
            {
                var (items, totalCount) = await _uploadService.GetPagedFilesAsync(
                    pageNumber, pageSize, siteId, userId, fileType, searchTerm);

                var response = new
                {
                    items,
                    totalCount,
                    pageNumber,
                    pageSize,
                    totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Dosyalar listelenirken bir hata oluştu.", details = ex.Message });
            }
        }

        /// Belirtilen site ID'sine ait dosyaları listeler
        [HttpGet("site/{siteId}")]
        public async Task<ActionResult<IEnumerable<UploadFileDto>>> GetFilesBySite(int siteId)
        {
            try
            {
                var files = await _uploadService.GetFilesBySiteIdAsync(siteId);
                return Ok(files);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Site dosyaları getirilirken bir hata oluştu.", details = ex.Message });
            }
        }

        /// Belirtilen kullanıcı ID'sine ait dosyaları listeler
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<UploadFileDto>>> GetFilesByUser(int userId)
        {
            try
            {
                var files = await _uploadService.GetFilesByUserIdAsync(userId);
                return Ok(files);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Kullanıcı dosyaları getirilirken bir hata oluştu.", details = ex.Message });
            }
        }

        /// Dosyayı siler (soft delete)
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFile(int id)
        {
            try
            {
                await _uploadService.DeleteFileAsync(id);
                return Ok(new { message = "Dosya başarıyla silindi." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Dosya silinirken bir hata oluştu.", details = ex.Message });
            }
        }

        /// Dosyayı fiziksel olarak siler
        [HttpDelete("{id}/permanent")]
        public async Task<ActionResult> DeleteFilePermanently(int id)
        {
            try
            {
                await _uploadService.DeleteFilePhysicallyAsync(id);
                return Ok(new { message = "Dosya kalıcı olarak silindi." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Dosya kalıcı olarak silinirken bir hata oluştu.", details = ex.Message });
            }
        }

        /// Upload app ayarlarını getirir
        [HttpGet("settings/{key}")]
        public async Task<ActionResult<UploadAppDto>> GetUploadSettings(string key)
        {
            try
            {
                var settings = await _uploadService.GetUploadAppByKeyAsync(key);
                
                if (settings == null)
                    return NotFound(new { message = $"Upload ayarları bulunamadı: {key}" });

                return Ok(settings);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Upload ayarları getirilirken bir hata oluştu.", details = ex.Message });
            }
        }

        /// Upload app ayarlarını oluşturur
        [HttpPost("settings")]
        public async Task<ActionResult<UploadAppDto>> CreateUploadSettings([FromBody] UploadAppDto uploadAppDto)
        {
            try
            {
                var result = await _uploadService.CreateUploadAppAsync(uploadAppDto);
                return CreatedAtAction(nameof(GetUploadSettings), new { key = result.Key }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Upload ayarları oluşturulurken bir hata oluştu.", details = ex.Message });
            }
        }

        /// Upload app ayarlarını günceller
        [HttpPut("settings/{id}")]
        public async Task<ActionResult<UploadAppDto>> UpdateUploadSettings(int id, [FromBody] UploadAppDto uploadAppDto)
        {
            try
            {
                uploadAppDto.Id = id;
                var result = await _uploadService.UpdateUploadAppAsync(uploadAppDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Upload ayarları güncellenirken bir hata oluştu.", details = ex.Message });
            }
        }

        /// Dosya türü kontrolü endpoint'i
        [HttpPost("validate/filetype")]
        public ActionResult<bool> ValidateFileType([FromBody] FileValidationRequest request)
        {
            try
            {
                var isAllowed = _uploadService.IsFileTypeAllowed(request.FileName, request.AllowedTypes);
                return Ok(new { isAllowed, fileName = request.FileName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Dosya türü kontrolü sırasında bir hata oluştu.", details = ex.Message });
            }
        }

        /// Dosya boyutu kontrolü endpoint'i
        [HttpPost("validate/filesize")]
        public ActionResult<bool> ValidateFileSize([FromBody] FileSizeValidationRequest request)
        {
            try
            {
                var isAllowed = _uploadService.IsFileSizeAllowed(request.FileSize, request.MaxSize);
                return Ok(new { isAllowed, fileSize = request.FileSize, maxSize = request.MaxSize });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Dosya boyutu kontrolü sırasında bir hata oluştu.", details = ex.Message });
            }
        }
    }

    /// Dosya türü doğrulama isteği
    public class FileValidationRequest
    {
        [Required]
        public string FileName { get; set; } = string.Empty;
        
        public string? AllowedTypes { get; set; }
    }

    /// Dosya boyutu doğrulama isteği
    public class FileSizeValidationRequest
    {
        [Required]
        public long FileSize { get; set; }
        
        public long? MaxSize { get; set; }
    }
} 