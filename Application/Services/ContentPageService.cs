using AutoMapper;
using Microsoft.EntityFrameworkCore;
using new_cms.Application.DTOs.ContentPageDTOs;
using new_cms.Application.Interfaces;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace new_cms.Application.Services
{
    /// Content sayfaları (TAppContentpage) yönetimi ile ilgili işlemleri gerçekleştiren servis sınıfı
    public class ContentPageService : IContentPageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IIdGeneratorService _idGenerator;

        public ContentPageService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IIdGeneratorService idGenerator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _idGenerator = idGenerator;
        }

        /// Sayfalı ve filtrelenmiş content sayfası listesi getirir
        public async Task<(IEnumerable<ContentPageListDto> Items, int TotalCount)> GetPagedContentPagesAsync(
            int pageNumber, 
            int pageSize, 
            int? groupId = null,
            int? siteId = null, 
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            try
            {
                var query = _unitOfWork.Repository<TAppContentpage>().Query()
                    .Where(cp => cp.IsDeleted == 0); // Soft delete filtresi

                // Group ID filtreleme
                if (groupId.HasValue && groupId.Value > 0)
                {
                    query = query.Where(cp => cp.Groupid == groupId.Value);
                }

                // Site ID filtreleme  
                if (siteId.HasValue && siteId.Value > 0)
                {
                    query = query.Where(cp => cp.Siteid == siteId.Value);
                }

                // Arama terimi filtreleme
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(cp => 
                        (cp.Header != null && cp.Header.Contains(searchTerm)) ||
                        (cp.Content != null && cp.Content.Contains(searchTerm))
                    );
                }

                // Sıralama uygulama - ORDERBY alanını kullanıyoruz
                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    query = sortBy.ToLowerInvariant() switch 
                    {
                        "id" => ascending ? query.OrderBy(cp => cp.Id) : query.OrderByDescending(cp => cp.Id),
                        "header" => ascending ? query.OrderBy(cp => cp.Header) : query.OrderByDescending(cp => cp.Header),
                        "orderby" => ascending ? query.OrderBy(cp => cp.Orderby) : query.OrderByDescending(cp => cp.Orderby),
                        "createddate" => ascending ? query.OrderBy(cp => cp.Createddate) : query.OrderByDescending(cp => cp.Createddate),
                        "modifieddate" => ascending ? query.OrderBy(cp => cp.Modifieddate) : query.OrderByDescending(cp => cp.Modifieddate),
                        _ => ascending ? query.OrderBy(cp => cp.Orderby ?? int.MaxValue).ThenBy(cp => cp.Id) : query.OrderByDescending(cp => cp.Orderby ?? int.MinValue).ThenByDescending(cp => cp.Id)
                    };
                }
                else
                {
                    // Varsayılan sıralama: OrderBy alanına göre, sonra ID'ye göre
                    query = ascending ? query.OrderBy(cp => cp.Orderby ?? int.MaxValue).ThenBy(cp => cp.Id) : query.OrderByDescending(cp => cp.Orderby ?? int.MinValue).ThenByDescending(cp => cp.Id);
                }

                var totalCount = await query.CountAsync();
                var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                return (_mapper.Map<IEnumerable<ContentPageListDto>>(items), totalCount);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Content sayfaları listelenirken bir hata oluştu.", ex);
            }
        }

        public async Task<ContentPageDto?> GetContentPageByIdAsync(int id)
        {
            if (id <= 0) {
                 throw new ArgumentException("Geçerli bir content sayfası ID'si gereklidir.", nameof(id));
            }

            try
            {
                var contentPage = await _unitOfWork.Repository<TAppContentpage>().Query()
                                .FirstOrDefaultAsync(cp => cp.Id == id && cp.IsDeleted == 0);
                
                return contentPage == null ? null : _mapper.Map<ContentPageDto>(contentPage);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Content sayfası getirilirken bir hata oluştu (ID: {id}).", ex);
            }
        }

        public async Task<ContentPageDto> CreateContentPageAsync(ContentPageDto contentPageDto)
        {
            if (contentPageDto == null) {
                 throw new ArgumentNullException(nameof(contentPageDto), "Oluşturulacak content sayfası bilgileri boş olamaz.");
            }
            if (string.IsNullOrWhiteSpace(contentPageDto.Header)) {
                 throw new ArgumentException("Content sayfası başlığı boş olamaz.", nameof(contentPageDto.Header));
            }
            if (contentPageDto.GroupId <= 0) { 
                 throw new ArgumentException("Geçerli bir Group ID'si gereklidir.", nameof(contentPageDto.GroupId));
            }

            try
            {
                var contentPage = _mapper.Map<TAppContentpage>(contentPageDto);
                 
                contentPage.Id = await _idGenerator.GenerateNextIdAsync<TAppContentpage>();
                contentPage.IsDeleted = 0; 
                contentPage.Createddate = DateTime.UtcNow; 
                // contentPage.Createduser = GetCurrentUserId(); // TODO: Aktif kullanıcı ID'si entegre edilmeli

                var createdContentPage = await _unitOfWork.Repository<TAppContentpage>().AddAsync(contentPage);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<ContentPageDto>(createdContentPage);
            }
            catch (DbUpdateException ex) {
                throw new InvalidOperationException($"Content sayfası oluşturulurken veritabanı hatası: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException || ex is ArgumentException) throw;
                throw new InvalidOperationException("Content sayfası oluşturulurken beklenmedik bir hata oluştu.", ex);
            }
        }

        public async Task<ContentPageDto> UpdateContentPageAsync(ContentPageDto contentPageDto)
        {
            if (contentPageDto?.Id == null || contentPageDto.Id <= 0)
                 throw new ArgumentNullException(nameof(contentPageDto), "Güncelleme için geçerli bir Content Sayfası ID'si gereklidir.");
            if (string.IsNullOrWhiteSpace(contentPageDto.Header)) {
                 throw new ArgumentException("Content sayfası başlığı boş olamaz.", nameof(contentPageDto.Header));
            }
            if (contentPageDto.GroupId <= 0) { 
                 throw new ArgumentException("Geçerli bir Group ID'si gereklidir.", nameof(contentPageDto.GroupId));
            }

            try
            {
                var existingContentPage = await _unitOfWork.Repository<TAppContentpage>().GetByIdAsync(contentPageDto.Id.Value);
                
                if (existingContentPage == null || existingContentPage.IsDeleted == 1)
                    throw new KeyNotFoundException($"Güncellenecek content sayfası bulunamadı veya silinmiş: ID {contentPageDto.Id.Value}");

                // Sistem tarafından yönetilen alanları sakla
                var originalIsDeleted = existingContentPage.IsDeleted;
                var originalCreatedDate = existingContentPage.Createddate;
                var originalCreatedUser = existingContentPage.Createduser;

                _mapper.Map(contentPageDto, existingContentPage);

                existingContentPage.IsDeleted = originalIsDeleted;
                existingContentPage.Createddate = originalCreatedDate;
                existingContentPage.Createduser = originalCreatedUser;

                // Güncelleme bilgilerini ayarla
                existingContentPage.Modifieddate = DateTime.UtcNow;
                // existingContentPage.Modifieduser = GetCurrentUserId(); // TODO: Aktif kullanıcı ID'si eklenmeli

                await _unitOfWork.Repository<TAppContentpage>().UpdateAsync(existingContentPage);
                await _unitOfWork.CompleteAsync();
                
                return _mapper.Map<ContentPageDto>(existingContentPage);
            }
            catch (DbUpdateException ex) {
                 throw new InvalidOperationException($"Content sayfası güncellenirken veritabanı hatası (ID: {contentPageDto.Id.Value}): {ex.InnerException?.Message ?? ex.Message}", ex);
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException || ex is ArgumentNullException || ex is ArgumentException) throw;
                throw new InvalidOperationException($"Content sayfası güncellenirken beklenmedik bir hata oluştu (ID: {contentPageDto.Id.Value}).", ex);
            }
        }

        public async Task DeleteContentPageAsync(int id)
        {
            if (id <= 0) {
                 throw new ArgumentException("Geçerli bir content sayfası ID'si gereklidir.", nameof(id));
            }

            try
            {
                await _unitOfWork.Repository<TAppContentpage>().SoftDeleteAsync(id);
                await _unitOfWork.CompleteAsync();
            }
            catch (KeyNotFoundException ex) 
            {
                throw new KeyNotFoundException($"Silinecek content sayfası bulunamadı veya zaten silinmiş: ID {id}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Content sayfası silinirken bir hata oluştu (ID: {id}).", ex);
            }
        }

        /// Belirtilen group'a ait content sayfalarını OrderBy alanına göre sıralı olarak getirir
        public async Task<IEnumerable<ContentPageListDto>> GetContentPagesByGroupIdAsync(int groupId)
        {
            if (groupId <= 0)
            {
                throw new ArgumentException("Geçerli bir Group ID'si gereklidir.", nameof(groupId));
            }

            try
            {
                var contentPages = await _unitOfWork.Repository<TAppContentpage>().Query()
                    .Where(cp => cp.Groupid == groupId && cp.IsDeleted == 0)
                    .OrderBy(cp => cp.Orderby ?? int.MaxValue).ThenBy(cp => cp.Id) // ORDERBY alanına göre sıralama
                    .ToListAsync();
                    
                return _mapper.Map<IEnumerable<ContentPageListDto>>(contentPages);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Group content sayfaları listelenirken hata oluştu (Group ID: {groupId}).", ex);
            }
        }

        /// Sistemdeki tüm aktif content sayfalarını getirir
        public async Task<IEnumerable<ContentPageListDto>> GetActiveContentPagesAsync()
        {
            try
            {
                var activeContentPages = await _unitOfWork.Repository<TAppContentpage>().Query()
                    .Where(cp => cp.IsDeleted == 0)
                    .OrderBy(cp => cp.Orderby ?? int.MaxValue).ThenBy(cp => cp.Id) // ORDERBY alanına göre sıralama
                    .ToListAsync();
                    
                return _mapper.Map<IEnumerable<ContentPageListDto>>(activeContentPages);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Aktif content sayfaları listelenirken bir hata oluştu.", ex);
            }
        }

        /// Belirtilen site'ye ait content sayfalarını getirir
        public async Task<IEnumerable<ContentPageListDto>> GetContentPagesBySiteIdAsync(int siteId)
        {
            if (siteId <= 0)
            {
                throw new ArgumentException("Geçerli bir Site ID'si gereklidir.", nameof(siteId));
            }

            try
            {
                var contentPages = await _unitOfWork.Repository<TAppContentpage>().Query()
                    .Where(cp => cp.Siteid == siteId && cp.IsDeleted == 0)
                    .OrderBy(cp => cp.Orderby ?? int.MaxValue).ThenBy(cp => cp.Id) // ORDERBY alanına göre sıralama
                    .ToListAsync();
                    
                return _mapper.Map<IEnumerable<ContentPageListDto>>(contentPages);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Site content sayfaları listelenirken hata oluştu (Site ID: {siteId}).", ex);
            }
        }
    }
} 