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
    /// Bileşen yönetimi ile ilgili işlemleri gerçekleştiren servis sınıfı.
    public class ComponentService : IComponentService
    {
        private readonly IRepository<TAppThemecomponent> _themeComponentRepository;
        private readonly IRepository<TAppSitecomponentdata> _siteComponentDataRepository;
        private readonly IMapper _mapper;

        public ComponentService(
            IRepository<TAppComponent> componentRepository,
            IRepository<TAppThemecomponent> themeComponentRepository,
            IRepository<TAppSitecomponentdata> siteComponentDataRepository,
            IMapper mapper)
        {
            _themeComponentRepository = themeComponentRepository;
            _siteComponentDataRepository = siteComponentDataRepository;
            _mapper = mapper;
        }

        /// Belirtilen tema-bileşen ilişkisini pasif hale getirir (soft delete).
        public async Task RemoveComponentFromThemeAsync(int themeComponentId)
        {
            try
            {
                await _themeComponentRepository.SoftDeleteAsync(themeComponentId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Tema-Bileşen ilişkisi kaldırılırken bir hata oluştu (ID: {themeComponentId}).", ex);
            }
        }

        /// Belirtilen ID'ye sahip aktif site bileşen verisini getirir.
        public async Task<SiteComponentDataDto?> GetComponentDataAsync(int siteComponentDataId)
        {
            try
            {
                var data = await _siteComponentDataRepository.Query()
                    .Include(scd => scd.Themecomponent) 
                    .FirstOrDefaultAsync(scd => scd.Id == siteComponentDataId && scd.Isdeleted == 0);

                if (data == null) return null;
                
                var dto = _mapper.Map<SiteComponentDataDto>(data);

                 if (data.Themecomponent != null)
                 {
                    dto.ComponentName = data.Themecomponent.Name; 
                 }

                return dto;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Site bileşen verisi getirilirken bir hata oluştu (ID: {siteComponentDataId}).", ex);
            }
        }

        /// Mevcut bir site bileşen verisini günceller.
        /// Sadece 'Data' alanı ve güncelleme bilgileri değiştirilir.
        public async Task<SiteComponentDataDto> UpdateComponentDataAsync(SiteComponentDataDto componentDataDto)
        {
             if (componentDataDto?.Id == null || componentDataDto.Id <= 0)
                throw new ArgumentNullException(nameof(componentDataDto), "Güncelleme için geçerli bir site bileşen veri ID'si gereklidir.");
            
            try
            {
                 var existingData = await _siteComponentDataRepository.Query()
                                           .Include(scd => scd.Themecomponent)
                                           .FirstOrDefaultAsync(scd => scd.Id == componentDataDto.Id.Value);
                                           
                if (existingData == null || existingData.Isdeleted == 1)
                    throw new KeyNotFoundException($"Güncellenecek site bileşen verisi bulunamadı veya silinmiş: ID {componentDataDto.Id.Value}");
                
                existingData.Data = componentDataDto.Data; 
                existingData.Modifieddate = DateTime.UtcNow;
                // existingData.Modifieduser = GetCurrentUserId(); 

                await _siteComponentDataRepository.UpdateAsync(existingData);
                
                 var updatedDto = _mapper.Map<SiteComponentDataDto>(existingData);
                 
                if (existingData.Themecomponent != null)
                {
                    updatedDto.ComponentName = existingData.Themecomponent.Name;
                }

                return updatedDto;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Site bileşen verisi güncellenirken veritabanı hatası oluştu (ID: {componentDataDto.Id.Value}).", ex);
            }
            catch (Exception ex)
            {
                 if (ex is KeyNotFoundException) throw;
                throw new InvalidOperationException($"Site bileşen verisi güncellenirken beklenmedik bir hata oluştu (ID: {componentDataDto.Id.Value}).", ex);
            }
        }

        /// Belirli bir site için kullanılması gereken tüm aktif bileşenleri ve verilerini getirir.
        public async Task<IEnumerable<SiteComponentDataDto>> GetComponentsForSiteAsync(int siteId)
        {
            try
            {
                var siteComponentData = await _siteComponentDataRepository.Query()
                    .Where(scd => scd.Siteid == siteId && scd.Isdeleted == 0)
                    .Include(scd => scd.Themecomponent) 
                    .ToListAsync();
                
                 var dtos = _mapper.Map<List<SiteComponentDataDto>>(siteComponentData);

                foreach (var dto in dtos)
                {
                    var correspondingEntity = siteComponentData.FirstOrDefault(scd => scd.Id == dto.Id);
                    if (correspondingEntity?.Themecomponent != null)
                    {
                        dto.ComponentName = correspondingEntity.Themecomponent.Name; 
                    }
                }

                return dtos;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Site için bileşenler getirilirken bir hata oluştu (Site ID: {siteId}).", ex);
            }
        }

        /// Bir bileşeni belirli bir temaya atar (yeni kayıt oluşturur).
        public async Task<ThemeComponentDto> AddComponentToThemeAsync(ThemeComponentDto themeComponentDto)
        {
            if (themeComponentDto == null)
                throw new ArgumentNullException(nameof(themeComponentDto));
            if (themeComponentDto.ThemeId <= 0)
                throw new ArgumentException("Geçerli bir Tema ID'si gereklidir.", nameof(themeComponentDto.ThemeId));
            if (themeComponentDto.ComponentId <= 0)
                throw new ArgumentException("Geçerli bir Bileşen ID'si gereklidir.", nameof(themeComponentDto.ComponentId));

            try
            {
                // Aynı tema ve bileşen için zaten aktif bir kayıt var mı kontrolü
                bool exists = await _themeComponentRepository.AnyAsync(tc => tc.Themeid == themeComponentDto.ThemeId 
                                                                        && tc.Componentid == themeComponentDto.ComponentId 
                                                                        && tc.Isdeleted == 0);
                if (exists)
                {
                    throw new InvalidOperationException("Bu bileşen zaten bu temaya atanmış.");
                }

                var themeComponentEntity = _mapper.Map<TAppThemecomponent>(themeComponentDto);
                themeComponentEntity.Isdeleted = 0;
                themeComponentEntity.Createddate = DateTime.UtcNow;
                // themeComponentEntity.Createduser = GetCurrentUserId(); 

                await _themeComponentRepository.AddAsync(themeComponentEntity);

                var resultDto = _mapper.Map<ThemeComponentDto>(themeComponentEntity);
                resultDto.ComponentName = themeComponentEntity.Name;
                
                return resultDto;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Tema-Bileşen ilişkisi eklenirken veritabanı hatası oluştu.", ex);
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException) throw;
                throw new InvalidOperationException("Tema-Bileşen ilişkisi eklenirken beklenmedik bir hata oluştu.", ex);
            }
        }

        /// Mevcut bir tema-bileşen ilişkisini günceller.
        public async Task<ThemeComponentDto> UpdateThemeComponentAsync(ThemeComponentDto themeComponentDto)
        {
            if (themeComponentDto == null)
                throw new ArgumentNullException(nameof(themeComponentDto));
            if (themeComponentDto.Id <= 0)
                throw new ArgumentException("Güncelleme için geçerli bir ID gereklidir.", nameof(themeComponentDto.Id));
            if (themeComponentDto.ThemeId <= 0)
                throw new ArgumentException("Geçerli bir Tema ID'si gereklidir.", nameof(themeComponentDto.ThemeId));
            if (themeComponentDto.ComponentId <= 0)
                throw new ArgumentException("Geçerli bir Bileşen ID'si gereklidir.", nameof(themeComponentDto.ComponentId));

            try
            {
                var themeComponentEntity = await _themeComponentRepository.GetByIdAsync(themeComponentDto.Id.Value);
                if (themeComponentEntity == null || themeComponentEntity.Isdeleted == 1)
                    throw new KeyNotFoundException($"Güncellenecek tema-bileşen ilişkisi bulunamadı: ID {themeComponentDto.Id}");
                
                var originalIsDeleted = themeComponentEntity.Isdeleted;
                var originalCreatedDate = themeComponentEntity.Createddate;
                var originalCreatedUser = themeComponentEntity.Createduser;

                _mapper.Map(themeComponentDto, themeComponentEntity);

                themeComponentEntity.Isdeleted = originalIsDeleted;
                themeComponentEntity.Createddate = originalCreatedDate;
                themeComponentEntity.Createduser = originalCreatedUser;
                themeComponentEntity.Modifieddate = DateTime.UtcNow;
                // themeComponentEntity.Modifieduser = GetCurrentUserId();

                await _themeComponentRepository.UpdateAsync(themeComponentEntity);

                var resultDto = _mapper.Map<ThemeComponentDto>(themeComponentEntity);
                resultDto.ComponentName = themeComponentEntity.Name;
                
                return resultDto;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Tema-Bileşen ilişkisi güncellenirken veritabanı hatası oluştu (ID: {themeComponentDto.Id}).", ex);
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException || ex is InvalidOperationException) throw;
                throw new InvalidOperationException($"Tema-Bileşen ilişkisi güncellenirken beklenmedik bir hata oluştu (ID: {themeComponentDto.Id}).", ex);
            }
        }
    }
} 