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
        private readonly IIdGeneratorService _idGenerator;

        public ComponentService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IIdGeneratorService idGenerator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _idGenerator = idGenerator;
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
                
                themeComponentEntity.Id = await _idGenerator.GenerateNextIdAsync<TAppThemecomponent>();
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

        /// Belirtilen ID'ye sahip bileşeni getirir.
        public async Task<ComponentDto?> GetComponentByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Geçerli bir bileşen ID'si gereklidir.", nameof(id));
            }

            try
            {
                var component = await _unitOfWork.Repository<TAppComponent>().Query()
                    .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == 0);
                    
                return component == null ? null : _mapper.Map<ComponentDto>(component);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Bileşen getirilirken bir hata oluştu (ID: {id}).", ex);
            }
        }

        /// Yeni bir bileşen oluşturur.
        public async Task<ComponentDto> CreateComponentAsync(ComponentDto componentDto)
        {
            if (componentDto == null)
                throw new ArgumentNullException(nameof(componentDto));
            if (string.IsNullOrWhiteSpace(componentDto.Name))
                throw new ArgumentException("Bileşen adı gereklidir.", nameof(componentDto.Name));

            try
            {
                // Aynı isimde aktif bileşen var mı kontrol et
                bool exists = await _unitOfWork.Repository<TAppComponent>().AnyAsync(c =>
                    c.Name == componentDto.Name && c.IsDeleted == 0);

                if (exists)
                {
                    throw new InvalidOperationException("Bu isimde bir bileşen zaten mevcut ve aktif durumdadır.");
                }

                var componentEntity = _mapper.Map<TAppComponent>(componentDto);
                 
                componentEntity.Id = await _idGenerator.GenerateNextIdAsync<TAppComponent>();
                componentEntity.IsDeleted = 0;
                componentEntity.Createddate = DateTime.UtcNow;
                // componentEntity.Createduser = GetCurrentUserId(); // TODO: Aktif kullanıcı ID'si alınmalı

                var addedEntity = await _unitOfWork.Repository<TAppComponent>().AddAsync(componentEntity);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<ComponentDto>(addedEntity);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Bileşen oluşturulurken veritabanı hatası oluştu.", ex);
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException || ex is ArgumentException) throw;
                throw new InvalidOperationException("Bileşen oluşturulurken beklenmedik bir hata oluştu.", ex);
            }
        }

        /// Mevcut bir bileşeni günceller.
        public async Task<ComponentDto> UpdateComponentAsync(ComponentDto componentDto)
        {
            if (componentDto == null)
                throw new ArgumentNullException(nameof(componentDto));
            if (componentDto.Id == null || componentDto.Id <= 0)
                throw new ArgumentException("Güncelleme için geçerli bir ID gereklidir.", nameof(componentDto.Id));
            if (string.IsNullOrWhiteSpace(componentDto.Name))
                throw new ArgumentException("Bileşen adı gereklidir.", nameof(componentDto.Name));

            var componentId = componentDto.Id.Value;

            try
            {
                var existingComponent = await _unitOfWork.Repository<TAppComponent>().GetByIdAsync(componentId);

                if (existingComponent == null || existingComponent.IsDeleted == 1)
                    throw new KeyNotFoundException($"Güncellenecek bileşen bulunamadı veya silinmiş: ID {componentId}");

                // Aynı isimde başka aktif bileşen var mı kontrol et (kendisi hariç)
                bool exists = await _unitOfWork.Repository<TAppComponent>().AnyAsync(c =>
                    c.Name == componentDto.Name && c.IsDeleted == 0 && c.Id != componentId);

                if (exists)
                {
                    throw new InvalidOperationException("Bu isimde başka bir aktif bileşen mevcut.");
                }

                // Orijinal değerleri sakla
                var originalIsDeleted = existingComponent.IsDeleted;
                var originalCreatedDate = existingComponent.Createddate;
                var originalCreatedUser = existingComponent.Createduser;

                // DTO'dan entity'ye map et
                _mapper.Map(componentDto, existingComponent);

                // Orijinal değerleri geri yükle
                existingComponent.IsDeleted = originalIsDeleted;
                existingComponent.Createddate = originalCreatedDate;
                existingComponent.Createduser = originalCreatedUser;
                existingComponent.Modifieddate = DateTime.UtcNow;
                // existingComponent.Modifieduser = GetCurrentUserId(); // TODO: Aktif kullanıcı ID'si alınmalı

                await _unitOfWork.Repository<TAppComponent>().UpdateAsync(existingComponent);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<ComponentDto>(existingComponent);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Bileşen güncellenirken veritabanı hatası oluştu (ID: {componentId}).", ex);
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException || ex is InvalidOperationException || ex is ArgumentException) throw;
                throw new InvalidOperationException($"Bileşen güncellenirken beklenmedik bir hata oluştu (ID: {componentId}).", ex);
            }
        }

        /// Belirtilen ID'ye sahip bileşeni pasif hale getirir (soft delete).
        public async Task DeleteComponentAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Geçerli bir bileşen ID'si gereklidir.", nameof(id));

            try
            {
                await _unitOfWork.Repository<TAppComponent>().SoftDeleteAsync(id);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Bileşen silinirken bir hata oluştu (ID: {id}).", ex);
            }
        }

        /// Tüm aktif bileşenleri listeler.
        public async Task<IEnumerable<ComponentDto>> GetAllComponentsAsync()
        {
            try
            {
                var components = await _unitOfWork.Repository<TAppComponent>().Query()
                    .Where(c => c.IsDeleted == 0)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<ComponentDto>>(components);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Bileşenler listelenirken bir hata oluştu.", ex);
            }
        }

        /// Tüm aktif tema-bileşen ilişkilerini listeler.
        public async Task<IEnumerable<ThemeComponentDto>> GetAllThemeComponentsAsync()
        {
            try
            {
                var themeComponents = await _unitOfWork.Repository<TAppThemecomponent>().Query()
                    .Where(tc => tc.Isdeleted == 0)
                    .OrderBy(tc => tc.Themeid)
                    .ThenBy(tc => tc.Componentid)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<ThemeComponentDto>>(themeComponents);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Tema-bileşen ilişkileri listelenirken bir hata oluştu.", ex);
            }
        }

        /// Belirtilen ID'ye sahip tema-bileşen ilişkisini getirir.
        public async Task<ThemeComponentDto?> GetThemeComponentByIdAsync(int themeComponentId)
        {
            if (themeComponentId <= 0)
                throw new ArgumentException("Geçerli bir tema-bileşen ID'si gereklidir.", nameof(themeComponentId));

            try
            {
                var themeComponent = await _unitOfWork.Repository<TAppThemecomponent>().Query()
                    .FirstOrDefaultAsync(tc => tc.Id == themeComponentId && tc.Isdeleted == 0);

                return themeComponent == null ? null : _mapper.Map<ThemeComponentDto>(themeComponent);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Tema-bileşen ilişkisi getirilirken bir hata oluştu (ID: {themeComponentId}).", ex);
            }
        }
    }
} 