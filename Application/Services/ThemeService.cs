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
        private readonly IRepository<TAppTheme> _themeRepository;
        private readonly IRepository<TAppThemecomponent> _themeComponentRepository;
        private readonly IMapper _mapper;

        public ThemeService(
            IRepository<TAppTheme> themeRepository,
            IRepository<TAppThemecomponent> themeComponentRepository,
            IMapper mapper)
        {
            _themeRepository = themeRepository;
            _themeComponentRepository = themeComponentRepository;
            _mapper = mapper;
        }

        /// Sistemdeki silinmemiş temaları listeler.
        public async Task<IEnumerable<ThemeDto>> GetAllThemesAsync()
        {
            try
            {
                var themes = await _themeRepository.Query()
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
                var theme = await _themeRepository.Query()
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

                var createdTheme = await _themeRepository.AddAsync(theme);

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

            try
            {
                var existingTheme = await _themeRepository.GetByIdAsync(themeDto.Id.Value);

                if (existingTheme == null || existingTheme.Isdeleted == 1)
                    throw new KeyNotFoundException($"Güncellenecek tema bulunamadı veya silinmiş: ID {themeDto.Id.Value}");

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

                await _themeRepository.UpdateAsync(existingTheme);

                return _mapper.Map<ThemeDto>(existingTheme);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Tema güncellenirken veritabanı hatası oluştu (ID: {themeDto.Id.Value}).", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Tema güncellenirken beklenmedik bir hata oluştu (ID: {themeDto.Id.Value}).", ex);
            }
        }

        /// Belirtilen ID'ye sahip temayı pasif hale getirir (soft delete).
        public async Task DeleteThemeAsync(int id)
        {
            try
            {
                await _themeRepository.SoftDeleteAsync(id);
            }
            catch (Exception ex)
            { 
                throw new InvalidOperationException($"Tema silinirken bir hata oluştu (ID: {id}).", ex);
            }
        }
    }
} 