using AutoMapper;
using Microsoft.EntityFrameworkCore;
using new_cms.Application.DTOs.ComponentDTOs;
using new_cms.Application.DTOs.ThemeDTOs;
using new_cms.Application.Interfaces;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace new_cms.Application.Services
{
    /// Tema yönetimi ile ilgili işlemleri gerçekleştiren servis sınıfı.
    public class ThemeService : IThemeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ThemeService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// Sistemdeki silinmemiş temaları listeler.
        public async Task<IEnumerable<ThemeDto>> GetAllThemesAsync()
        {
            try
            {
                var themes = await _unitOfWork.Repository<TAppTheme>().Query()
                    .Where(t => t.Isdeleted == 0)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<ThemeDto>>(themes);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Tema listesi alınırken bir hata oluştu.", ex);
            }
        }

        /// Belirtilen ID'ye sahip silinmemiş temayı getirir.
        /// "id">Getirilecek temanın ID'si.
        public async Task<ThemeDto?> GetThemeByIdAsync(int id)
        {
            try
            {
                var theme = await _unitOfWork.Repository<TAppTheme>().Query()
                    .FirstOrDefaultAsync(t => t.Id == id && t.Isdeleted == 0);

                if (theme == null)
                    return null;

                return _mapper.Map<ThemeDto>(theme);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Tema getirilirken bir hata oluştu (ID: {id}).", ex);
            }
        }

        /// Yeni bir tema oluşturur.
        public async Task<ThemeDto> CreateThemeAsync(ThemeDto themeDto)
        {
            try
            {
                var theme = _mapper.Map<TAppTheme>(themeDto);

                theme.Isdeleted = 0; 
                theme.Createddate = DateTime.UtcNow; 
                // theme.Createduser = GetCurrentUserId(); // TODO: Aktif kullanıcı ID'si alınmalı

                var createdTheme = await _unitOfWork.Repository<TAppTheme>().AddAsync(theme);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<ThemeDto>(createdTheme);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Tema oluşturulurken beklenmedik bir hata oluştu.", ex);
            }
        }

        /// Mevcut bir temanın bilgilerini günceller.
        public async Task<ThemeDto> UpdateThemeAsync(ThemeDto themeDto)
        {
            if (themeDto?.Id == null || themeDto.Id <= 0)
                throw new ArgumentNullException(nameof(themeDto), "Güncelleme için geçerli bir tema ID'si gereklidir.");

            var themeId = themeDto.Id.Value;

            try
            {
                var existingTheme = await _unitOfWork.Repository<TAppTheme>().GetByIdAsync(themeId);

                if (existingTheme == null || existingTheme.Isdeleted == 1)
                    throw new KeyNotFoundException($"Güncellenecek tema bulunamadı veya silinmiş: ID {themeId}");

                // AutoMapper'ın üzerine yazmaması gereken alanları sakla
                var originalIsDeleted = existingTheme.Isdeleted;
                var originalCreatedDate = existingTheme.Createddate;
                var originalCreatedUser = existingTheme.Createduser;

                _mapper.Map(themeDto, existingTheme);

                // Saklanan orijinal değerleri geri yükle
                existingTheme.Isdeleted = originalIsDeleted;
                existingTheme.Createddate = originalCreatedDate;
                existingTheme.Createduser = originalCreatedUser;

                existingTheme.Modifieddate = DateTime.UtcNow;
                // existingTheme.Modifieduser = GetCurrentUserId(); // TODO: Aktif kullanıcı ID'si alınmalı

                await _unitOfWork.Repository<TAppTheme>().UpdateAsync(existingTheme);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<ThemeDto>(existingTheme);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Tema güncellenirken veritabanı hatası oluştu (ID: {themeId}).", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Tema güncellenirken beklenmedik bir hata oluştu (ID: {themeId}).", ex);
            }
        }

        /// Belirtilen ID'ye sahip temayı pasif hale getirir (soft delete).
        public async Task DeleteThemeAsync(int id)
        {
            try
            {
                // Önce temanın var olup olmadığını kontrol et
                var themeToDelete = await _unitOfWork.Repository<TAppTheme>().GetByIdAsync(id);
                if (themeToDelete == null)
                {
                    throw new KeyNotFoundException($"Silinecek tema bulunamadı: ID {id}");
                }
                else if (themeToDelete.Isdeleted == 1)
                {
                    throw new KeyNotFoundException($"Silinecek tema zaten silinmiş: ID {id}");
                }
                
                // SoftDeleteAsync'i kullanarak silme işlemini gerçekleştir
                await _unitOfWork.Repository<TAppTheme>().SoftDeleteAsync(id);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException) throw;
                throw new InvalidOperationException($"Tema silinirken bir hata oluştu (ID: {id}).", ex);
            }
        }
    }
} 