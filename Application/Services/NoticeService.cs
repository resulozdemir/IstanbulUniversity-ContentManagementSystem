using AutoMapper;
using Microsoft.EntityFrameworkCore;
using new_cms.Application.DTOs.NoticeDTOs;
using new_cms.Application.Interfaces;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace new_cms.Application.Services
{ 
    /// Duyuru (TAppNotice) varlıkları ile ilgili işlemleri gerçekleştiren servis sınıfı. 
    public class NoticeService : INoticeService
    {
        // UnitOfWork ve AutoMapper bağımlılıkları
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
 
        /// NoticeService sınıfının yeni bir örneğini başlatır. 
        public NoticeService(
            IUnitOfWork unitOfWork, 
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<(IEnumerable<NoticeListDto> Items, int TotalCount)> GetPagedNoticesAsync(
            int pageNumber, int pageSize, int? siteId = null, string? searchTerm = null, string? sortBy = null, bool ascending = true)
        { 
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;  

            try
            { 
                var query = _unitOfWork.Repository<TAppNotice>().Query().Where(n => n.Isdeleted == 0);  
 
                if (siteId.HasValue && siteId.Value > 0)
                {
                    query = query.Where(n => n.Siteid == siteId.Value);
                }
 
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                     query = query.Where(n => 
                        (n.Header != null && n.Header.Contains(searchTerm)) || 
                        (n.Content != null && n.Content.Contains(searchTerm)) ||
                        (n.Tag != null && n.Tag.Contains(searchTerm))
                     );
                }
                
                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    query = sortBy.ToLowerInvariant() switch 
                    {
                        "id" => ascending ? query.OrderBy(n => n.Id) : query.OrderByDescending(n => n.Id),
                        "header" => ascending ? query.OrderBy(n => n.Header) : query.OrderByDescending(n => n.Header),
                        "date" => ascending ? query.OrderBy(n => n.Ondate) : query.OrderByDescending(n => n.Ondate),
                        "createddate" => ascending ? query.OrderBy(n => n.Createddate) : query.OrderByDescending(n => n.Createddate), 
                        "modifieddate" => ascending ? query.OrderBy(n => n.Modifieddate) : query.OrderByDescending(n => n.Modifieddate), 
                        "ispublish" => ascending ? query.OrderBy(n => n.Ispublish) : query.OrderByDescending(n => n.Ispublish), 
                        "categoryid" => ascending ? query.OrderBy(n => n.Categoryid) : query.OrderByDescending(n => n.Categoryid), 
                        _ => ascending ? query.OrderBy(n => n.Ondate) : query.OrderByDescending(n => n.Ondate) 
                    };
                }
                else
                {
                    query = ascending ? query.OrderBy(n => n.Ondate) : query.OrderByDescending(n => n.Ondate);
                }
                
                var totalCount = await query.CountAsync();
                
                var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                return (_mapper.Map<IEnumerable<NoticeListDto>>(items), totalCount);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Sayfalanmış duyurular listelenirken bir hata oluştu.", ex);
            }
        }

        public async Task<NoticeDto?> GetNoticeByIdAsync(int id)
        {
            if (id <= 0) {
                 throw new ArgumentException("Geçerli bir duyuru ID'si gereklidir.", nameof(id));
            }

            try
            {
                var notice = await _unitOfWork.Repository<TAppNotice>().Query()
                                    .FirstOrDefaultAsync(n => n.Id == id && n.Isdeleted == 0); 
                
                return notice == null ? null : _mapper.Map<NoticeDto>(notice);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Duyuru getirilirken bir hata oluştu (ID: {id}).", ex);
            }
        }

        public async Task<NoticeDto> CreateNoticeAsync(NoticeDto noticeDto)
        {
             if (noticeDto == null) {
                 throw new ArgumentNullException(nameof(noticeDto), "Oluşturulacak duyuru bilgileri boş olamaz.");
             }
             if (string.IsNullOrWhiteSpace(noticeDto.Header)) {
                 throw new ArgumentException("Duyuru başlığı boş olamaz.", nameof(noticeDto.Header));
             }
             if (noticeDto.SiteId <= 0) { 
                 throw new ArgumentException("Geçerli bir Site ID'si gereklidir.", nameof(noticeDto.SiteId));
             }

             try
            {
                var notice = _mapper.Map<TAppNotice>(noticeDto);
                notice.Isdeleted = 0;
                notice.Createddate = DateTime.UtcNow; 
                // notice.Createduser = GetCurrentUserId(); // TODO: Aktif kullanıcı ID'si entegre edilmeli

                var createdNotice = await _unitOfWork.Repository<TAppNotice>().AddAsync(notice);
                await _unitOfWork.CompleteAsync();
                
                return _mapper.Map<NoticeDto>(createdNotice);
            }
            catch (DbUpdateException ex) {
                throw new InvalidOperationException($"Duyuru oluşturulurken veritabanı hatası: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException || ex is ArgumentException) throw;
                throw new InvalidOperationException("Duyuru oluşturulurken beklenmedik bir hata oluştu.", ex);
            }
        }

        public async Task<NoticeDto> UpdateNoticeAsync(NoticeDto noticeDto)
        {
            if (noticeDto?.Id == null || noticeDto.Id <= 0)
                 throw new ArgumentNullException(nameof(noticeDto), "Güncelleme için geçerli bir Duyuru ID'si gereklidir.");
             if (string.IsNullOrWhiteSpace(noticeDto.Header)) {
                 throw new ArgumentException("Duyuru başlığı boş olamaz.", nameof(noticeDto.Header));
             }
             if (noticeDto.SiteId <= 0) { 
                 throw new ArgumentException("Geçerli bir Site ID'si gereklidir.", nameof(noticeDto.SiteId));
             }

             try
            {
                var existingNotice = await _unitOfWork.Repository<TAppNotice>().GetByIdAsync(noticeDto.Id); 
                
                // Duyuru var mı ve silinmemiş mi kontrolü
                if (existingNotice == null || existingNotice.Isdeleted == 1)
                    throw new KeyNotFoundException($"Güncellenecek duyuru bulunamadı veya silinmiş: ID {noticeDto.Id}");

                var originalIsDeleted = existingNotice.Isdeleted;
                var originalCreatedDate = existingNotice.Createddate;
                var originalCreatedUser = existingNotice.Createduser;
                // var originalSiteId = existingNotice.Siteid; // SiteID genellikle güncellenmez, gerekirse bu da korunmalı.

                _mapper.Map(noticeDto, existingNotice);

                existingNotice.Isdeleted = originalIsDeleted;
                existingNotice.Createddate = originalCreatedDate;
                existingNotice.Createduser = originalCreatedUser;
                // existingNotice.Siteid = originalSiteId; // Gerekirse

                // Güncelleme bilgilerini ayarla
                existingNotice.Modifieddate = DateTime.UtcNow;
                // existingNotice.Modifieduser = GetCurrentUserId(); // TODO: Aktif kullanıcı ID'si eklenmeli

                await _unitOfWork.Repository<TAppNotice>().UpdateAsync(existingNotice);
                await _unitOfWork.CompleteAsync();
                
                return _mapper.Map<NoticeDto>(existingNotice);
            }
            catch (DbUpdateException ex) {
                 throw new InvalidOperationException($"Duyuru güncellenirken veritabanı hatası (ID: {noticeDto.Id}): {ex.InnerException?.Message ?? ex.Message}", ex);
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException || ex is ArgumentNullException || ex is ArgumentException) throw;
                throw new InvalidOperationException($"Duyuru güncellenirken beklenmedik bir hata oluştu (ID: {noticeDto.Id}).", ex);
            }
        }

        public async Task DeleteNoticeAsync(int id)
        {
            if (id <= 0) {
                 throw new ArgumentException("Geçerli bir duyuru ID'si gereklidir.", nameof(id));
            }

            try
            {
                await _unitOfWork.Repository<TAppNotice>().SoftDeleteAsync(id); 
                await _unitOfWork.CompleteAsync();
            }
            catch (KeyNotFoundException ex) 
            {
                throw new KeyNotFoundException($"Silinecek duyuru bulunamadı veya zaten silinmiş: ID {id}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Duyuru silinirken bir hata oluştu (ID: {id}).", ex);
            }
        }
    }
} 