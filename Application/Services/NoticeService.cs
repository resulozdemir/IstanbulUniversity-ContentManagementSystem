using AutoMapper;
using Microsoft.EntityFrameworkCore;
using new_cms.Application.DTOs.NoticeDTOs;
using new_cms.Application.Interfaces;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace new_cms.Application.Services
{
    /// Duyuru (Notice) işlemleri için servis implementasyonu.
    public class NoticeService : INoticeService
    {
        private readonly IRepository<TAppNotice> _noticeRepository;
        private readonly IMapper _mapper;

        public NoticeService(
            IRepository<TAppNotice> noticeRepository,
            IMapper mapper)
        {
            _noticeRepository = noticeRepository;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<(IEnumerable<NoticeListDto> Items, int TotalCount)> GetPagedNoticesAsync(
            int pageNumber, int pageSize, int? siteId = null, string? searchTerm = null, string? sortBy = null, bool ascending = true)
        {
            try
            {
                // Silinmemiş duyuruları sorgula
                var query = _noticeRepository.Query().Where(n => n.Isdeleted == 0);

                // Site ID'sine göre filtrele
                if (siteId.HasValue)
                {
                    query = query.Where(n => n.Siteid == siteId.Value);
                }

                // Arama terimine göre filtrele (Başlık, İçerik, Etiket)
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                     query = query.Where(n => n.Header.Contains(searchTerm) || 
                                             (n.Content != null && n.Content.Contains(searchTerm)) ||
                                             (n.Tag != null && n.Tag.Contains(searchTerm)));
                }
                
                // Dinamik sıralama
                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    query = sortBy.ToLower() switch
                    {
                        "id" => ascending ? query.OrderBy(n => n.Id) : query.OrderByDescending(n => n.Id),
                        "header" => ascending ? query.OrderBy(n => n.Header) : query.OrderByDescending(n => n.Header),
                        "date" => ascending ? query.OrderBy(n => n.Ondate) : query.OrderByDescending(n => n.Ondate),
                        "created" => ascending ? query.OrderBy(n => n.Createddate) : query.OrderByDescending(n => n.Createddate),
                        "modified" => ascending ? query.OrderBy(n => n.Modifieddate) : query.OrderByDescending(n => n.Modifieddate),
                        "publish" => ascending ? query.OrderBy(n => n.Ispublish) : query.OrderByDescending(n => n.Ispublish),
                        "category" => ascending ? query.OrderBy(n => n.Categoryid) : query.OrderByDescending(n => n.Categoryid),
                        _ => ascending ? query.OrderBy(n => n.Ondate) : query.OrderByDescending(n => n.Ondate) // Varsayılan tarih sıralaması
                    };
                }
                else
                {
                    // Varsayılan sıralama (sortBy belirtilmemişse)
                    query = ascending ? query.OrderBy(n => n.Ondate) : query.OrderByDescending(n => n.Ondate);
                }
                
                var totalCount = await query.CountAsync();
                
                var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                return (_mapper.Map<IEnumerable<NoticeListDto>>(items), totalCount);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Duyurular listelenirken bir hata oluştu.", ex);
            }
        }

        /// <inheritdoc />
        public async Task<NoticeDto?> GetNoticeByIdAsync(int id)
        {
            try
            {
                var notice = await _noticeRepository.Query().FirstOrDefaultAsync(n => n.Id == id && n.Isdeleted == 0);
                
                return notice == null ? null : _mapper.Map<NoticeDto>(notice);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Duyuru getirilirken bir hata oluştu (ID: {id}).", ex);
            }
        }

        /// <inheritdoc />
        public async Task<NoticeDto> CreateNoticeAsync(NoticeDto noticeDto)
        {
             try
            {
                var notice = _mapper.Map<TAppNotice>(noticeDto);
                notice.Isdeleted = 0;
                notice.Createddate = DateTime.UtcNow;
                // notice.Createduser = GetCurrentUserId(); 

                var createdNotice = await _noticeRepository.AddAsync(notice);
                
                return _mapper.Map<NoticeDto>(createdNotice);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Duyuru oluşturulurken bir hata oluştu.", ex);
            }
        }

        /// <inheritdoc />
        public async Task<NoticeDto> UpdateNoticeAsync(NoticeDto noticeDto)
        {
            if (noticeDto?.Id == null || noticeDto.Id <= 0)
                 throw new ArgumentNullException(nameof(noticeDto), "Güncelleme için geçerli bir Duyuru ID'si gereklidir.");

             try
            {
                var existingNotice = await _noticeRepository.GetByIdAsync(noticeDto.Id);
                
                if (existingNotice == null || existingNotice.Isdeleted == 1)
                    throw new KeyNotFoundException($"Güncellenecek duyuru bulunamadı veya silinmiş: ID {noticeDto.Id}");

                // Orijinal değerlerini koru
                var originalIsDeleted = existingNotice.Isdeleted;
                var originalCreatedDate = existingNotice.Createddate;
                var originalCreatedUser = existingNotice.Createduser;

                _mapper.Map(noticeDto, existingNotice);

                // değerleri geri yükle
                existingNotice.Isdeleted = originalIsDeleted;
                existingNotice.Createddate = originalCreatedDate;
                existingNotice.Createduser = originalCreatedUser;
                existingNotice.Modifieddate = DateTime.UtcNow;
                // existingNotice.Modifieduser = GetCurrentUserId();

                await _noticeRepository.UpdateAsync(existingNotice);
                
                return _mapper.Map<NoticeDto>(existingNotice);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Duyuru güncellenirken bir hata oluştu (ID: {noticeDto.Id}).", ex);
            }
        }

        /// <inheritdoc />
        public async Task DeleteNoticeAsync(int id)
        {
            try
            {
                await _noticeRepository.SoftDeleteAsync(id); 
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