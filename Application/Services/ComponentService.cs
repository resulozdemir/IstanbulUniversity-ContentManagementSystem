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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ComponentService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// Belirtilen tema-bileşen ilişkisini pasif hale getirir (soft delete).
        public async Task RemoveComponentFromThemeAsync(int themeComponentId)
        {
            try
            {
                await _unitOfWork.Repository<TAppThemecomponent>().SoftDeleteAsync(themeComponentId);
                
                await _unitOfWork.CompleteAsync();
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
                var data = await _unitOfWork.Repository<TAppSitecomponentdata>().Query()
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

            var siteComponentDataId = componentDataDto.Id.Value;

            try
            {
                 var existingData = await _unitOfWork.Repository<TAppSitecomponentdata>().Query()
                                           .Include(scd => scd.Themecomponent) 
                                           .FirstOrDefaultAsync(scd => scd.Id == siteComponentDataId); 

                if (existingData == null || existingData.Isdeleted == 1)
                    throw new KeyNotFoundException($"Güncellenecek site bileşen verisi bulunamadı veya silinmiş: ID {siteComponentDataId}");

                existingData.Data = componentDataDto.Data;
                existingData.Modifieddate = DateTime.UtcNow; 
                // existingData.Modifieduser = GetCurrentUserId(); // TODO: Aktif kullanıcı ID'si alınmalı

                await _unitOfWork.Repository<TAppSitecomponentdata>().UpdateAsync(existingData);
                
                await _unitOfWork.CompleteAsync();

                
                var updatedDto = _mapper.Map<SiteComponentDataDto>(existingData);

                if (existingData.Themecomponent != null)
                {
                    updatedDto.ComponentName = existingData.Themecomponent.Name;
                }

                return updatedDto;
            }
            catch (DbUpdateException ex) 
            {
                throw new InvalidOperationException($"Site bileşen verisi güncellenirken veritabanı hatası oluştu (ID: {siteComponentDataId}).", ex);
            }
            catch (Exception ex) 
            {
                 if (ex is KeyNotFoundException) throw; 
                throw new InvalidOperationException($"Site bileşen verisi güncellenirken beklenmedik bir hata oluştu (ID: {siteComponentDataId}).", ex);
            }
        }

        /// Belirli bir site için kullanılması gereken tüm aktif bileşenleri ve verilerini getirir.
        public async Task<IEnumerable<SiteComponentDataDto>> GetComponentsForSiteAsync(int siteId)
        {
            try
            {
                var siteComponentData = await _unitOfWork.Repository<TAppSitecomponentdata>().Query()
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

        /// Bir bileşeni belirli bir temaya atar (yeni TAppThemecomponent kaydı oluşturur).
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
                bool exists = await _unitOfWork.Repository<TAppThemecomponent>().AnyAsync(tc =>
                    tc.Themeid == themeComponentDto.ThemeId &&
                    tc.Componentid == themeComponentDto.ComponentId &&
                    tc.Isdeleted == 0); 

                if (exists)
                {
                    throw new InvalidOperationException("Bu bileşen zaten bu temaya atanmış ve aktif durumdadır.");
                }

                var themeComponentEntity = _mapper.Map<TAppThemecomponent>(themeComponentDto);
                themeComponentEntity.Isdeleted = 0; 
                themeComponentEntity.Createddate = DateTime.UtcNow; 
                // themeComponentEntity.Createduser = GetCurrentUserId(); // TODO: Aktif kullanıcı ID'si alınmalı

                var addedEntity = await _unitOfWork.Repository<TAppThemecomponent>().AddAsync(themeComponentEntity);

                await _unitOfWork.CompleteAsync();

                var resultDto = _mapper.Map<ThemeComponentDto>(addedEntity);

                resultDto.Name = themeComponentDto.Name; 
                resultDto.Name = themeComponentDto.Name; // Map yerine DTO'dan alınabilir veya sorgu ile çekilebilir

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

        /// Mevcut bir tema-bileşen ilişkisini (TAppThemecomponent) günceller.
        public async Task<ThemeComponentDto> UpdateThemeComponentAsync(ThemeComponentDto themeComponentDto)
        {
            if (themeComponentDto == null)
                throw new ArgumentNullException(nameof(themeComponentDto));
            if (themeComponentDto.Id == null || themeComponentDto.Id <= 0) 
                throw new ArgumentException("Güncelleme için geçerli bir ID gereklidir.", nameof(themeComponentDto.Id));
             if (themeComponentDto.ThemeId <= 0)
                 throw new ArgumentException("Geçerli bir Tema ID'si gereklidir.", nameof(themeComponentDto.ThemeId));
             if (themeComponentDto.ComponentId <= 0)
                 throw new ArgumentException("Geçerli bir Bileşen ID'si gereklidir.", nameof(themeComponentDto.ComponentId));

             var themeComponentId = themeComponentDto.Id.Value;

            try
            {
                var themeComponentEntity = await _unitOfWork.Repository<TAppThemecomponent>().GetByIdAsync(themeComponentId);

                if (themeComponentEntity == null || themeComponentEntity.Isdeleted == 1)
                    throw new KeyNotFoundException($"Güncellenecek tema-bileşen ilişkisi bulunamadı veya silinmiş: ID {themeComponentId}");

                var originalIsDeleted = themeComponentEntity.Isdeleted;
                var originalCreatedDate = themeComponentEntity.Createddate;
                var originalCreatedUser = themeComponentEntity.Createduser;

                _mapper.Map(themeComponentDto, themeComponentEntity);

                themeComponentEntity.Isdeleted = originalIsDeleted;
                themeComponentEntity.Createddate = originalCreatedDate;
                themeComponentEntity.Createduser = originalCreatedUser;
                themeComponentEntity.Modifieddate = DateTime.UtcNow; 
                // themeComponentEntity.Modifieduser = GetCurrentUserId(); // TODO: Aktif kullanıcı ID'si alınmalı

                await _unitOfWork.Repository<TAppThemecomponent>().UpdateAsync(themeComponentEntity);

                await _unitOfWork.CompleteAsync();

                var resultDto = _mapper.Map<ThemeComponentDto>(themeComponentEntity);

                resultDto.Name = themeComponentDto.Name; 

                return resultDto;
            }
            catch (DbUpdateException ex) 
            {
                throw new InvalidOperationException($"Tema-Bileşen ilişkisi güncellenirken veritabanı hatası oluştu (ID: {themeComponentId}).", ex);
            }
            catch (Exception ex) 
            {
                if (ex is KeyNotFoundException || ex is InvalidOperationException) throw; 
                throw new InvalidOperationException($"Tema-Bileşen ilişkisi güncellenirken beklenmedik bir hata oluştu (ID: {themeComponentId}).", ex);
            }
        }
    }
} 