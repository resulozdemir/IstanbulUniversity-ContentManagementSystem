using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using new_cms.Application.DTOs.ComponentDTOs;
using new_cms.Application.Interfaces;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;

namespace new_cms.Application.Services
{
    public class ComponentService : IComponentService
    {
        private readonly IComponentRepository _componentRepository;
        private readonly IMapper _mapper;

        public ComponentService(IComponentRepository componentRepository, IMapper mapper)
        {
            _componentRepository = componentRepository ?? throw new ArgumentNullException(nameof(componentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // Tüm bileşenleri listele
        public async Task<IEnumerable<ComponentDto>> GetAllComponentsAsync()
        {
            // Tüm bileşenleri getir
            var components = await _componentRepository.GetAllComponentsAsync();
            
            // DTO'ya dönüştür
            return _mapper.Map<IEnumerable<ComponentDto>>(components);
        }

        // Bileşen detayını getir
        public async Task<ComponentDto?> GetComponentByIdAsync(int id)
        {
            // Bileşen bilgisini getir
            var component = await _componentRepository.GetComponentByIdAsync(id);
            if (component == null)
                return null;
                
            // DTO'ya dönüştür
            return _mapper.Map<ComponentDto>(component);
        }

        // Yeni bileşen oluştur
        public async Task<ComponentDto> CreateComponentAsync(ComponentDto componentDto)
        {
            // DTO'yu entity'e dönüştür
            var component = _mapper.Map<TAppComponent>(componentDto);
            
            // Repository üzerinden kaydet
            var createdComponent = await _componentRepository.AddAsync(component);
            
            // Kaydedilen entity'yi DTO'ya dönüştür ve geri döndür
            return _mapper.Map<ComponentDto>(createdComponent);
        }

        // Bileşen bilgilerini güncelle
        public async Task<ComponentDto> UpdateComponentAsync(ComponentDto componentDto)
        {
            // Güncellenecek bileşen ID kontrol et
            if (!componentDto.Id.HasValue)
                throw new ArgumentException("Component ID is required for update operation");
            
            // Mevcut bileşen verilerini getir
            var existingComponent = await _componentRepository.GetByIdAsync(componentDto.Id.Value);
            if (existingComponent == null)
                throw new KeyNotFoundException($"Component with ID {componentDto.Id.Value} not found");
            
            // DTO'dan entity'ye dönüştür
            var component = _mapper.Map<ComponentDto, TAppComponent>(componentDto, existingComponent);
            
            // Repository üzerinden güncelle
            var updatedComponent = await _componentRepository.UpdateAsync(component);
            
            // Güncellenen entity'yi DTO'ya dönüştür ve geri döndür
            return _mapper.Map<ComponentDto>(updatedComponent);
        }

        // Bileşen sil (soft delete)
        public async Task DeleteComponentAsync(int id)
        {
            // Bileşenin kullanımda olup olmadığını kontrol et
            if (await IsComponentInUseAsync(id))
                throw new InvalidOperationException($"Component with ID {id} is in use and cannot be deleted");
                
            // Soft delete kullanarak bileşeni sil
            await _componentRepository.SoftDeleteAsync(id);
        }

        // Belirli türdeki bileşenleri listele
        public async Task<IEnumerable<ComponentDto>> GetComponentsByTypeAsync(string type)
        {
            // Belirtilen türdeki bileşenleri getir
            var components = await _componentRepository.GetComponentsByTypeAsync(type);
            
            // DTO'ya dönüştür
            return _mapper.Map<IEnumerable<ComponentDto>>(components);
        }

        // Bileşenin kullanımda olup olmadığını kontrol et
        public async Task<bool> IsComponentInUseAsync(int id)
        {
            return await _componentRepository.IsComponentInUseAsync(id);
        }

        // Siteye ait bileşen verilerini listele
        public async Task<IEnumerable<SiteComponentDataDto>> GetComponentDataBySiteIdAsync(int siteId)
        {
            // Site bileşen verilerini getir
            var componentData = await _componentRepository.GetComponentDataBySiteIdAsync(siteId);
            
            // DTO'ya dönüştür
            var dataDtos = _mapper.Map<IEnumerable<SiteComponentDataDto>>(componentData);
            
            // Bileşen adlarını doldur
            foreach (var data in dataDtos)
            {
                if (data.ComponentId > 0)
                {
                    var component = await _componentRepository.GetComponentByIdAsync(data.ComponentId.GetValueOrDefault());
                    if (component != null)
                    {
                        data.ComponentName = component.Name;
                    }
                }
            }
            
            return dataDtos;
        }

        // Bileşen verisi detayını getir
        public async Task<SiteComponentDataDto?> GetComponentDataByIdAsync(int dataId)
        {
            // Bileşen veri detayını getir
            var data = await _componentRepository.GetComponentDataByIdAsync(dataId);
            if (data == null)
                return null;
                
            // DTO'ya dönüştür
            var dataDto = _mapper.Map<SiteComponentDataDto>(data);
            
            // Bileşen adını doldur
            if (dataDto.ComponentId > 0) // Nullable kontrolü yerine değer kontrolü yapıyoruz
            {
                var component = await _componentRepository.GetComponentByIdAsync(dataDto.ComponentId.Value);
                if (component != null)
                {
                    dataDto.ComponentName = component.Name;
                }
            }
            
            return dataDto;
        }

        // Siteye bileşen verisi ekle
        public async Task<SiteComponentDataDto> AddComponentDataToSiteAsync(SiteComponentDataDto dataDto)
        {
            try
            {
                // DTO'yu entity'e dönüştür
                var data = _mapper.Map<TAppSitecomponentdata>(dataDto);
                
                // İlişkileri doğru şekilde ayarla
                data.Siteid = dataDto.SiteId;
                data.Themecomponentid = dataDto.ThemeComponentId;
                data.Createddate = DateTime.Now;
                
                
                var result = _mapper.Map<SiteComponentDataDto>(dataDto);
                
                // Bileşen adını doldur
                if (result.ComponentId.GetValueOrDefault() > 0)
                {
                    var component = await _componentRepository.GetComponentByIdAsync(result.ComponentId.GetValueOrDefault());
                    if (component != null)
                    {
                        result.ComponentName = component.Name;
                    }
                }
                
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Site bileşen verisi eklenirken hata: {ex.Message}", ex);
            }
        }

        // Bileşen verisini güncelle
        public async Task<SiteComponentDataDto> UpdateComponentDataAsync(SiteComponentDataDto dataDto)
        {
            try
            {
                // Güncellenecek veri ID kontrol et
                if (!dataDto.Id.HasValue)
                    throw new ArgumentException("Component data ID is required for update operation");
                
                // Mevcut bileşen veri detayını getir
                var existingData = await _componentRepository.GetComponentDataByIdAsync(dataDto.Id.Value);
                if (existingData == null)
                    throw new KeyNotFoundException($"Component data with ID {dataDto.Id.Value} not found");
                
                // DTO'dan entity'ye dönüştür
                var data = _mapper.Map<SiteComponentDataDto, TAppSitecomponentdata>(dataDto, existingData);
                
                // İlişkileri ve zaman bilgilerini güncelle
                data.Siteid = dataDto.SiteId;
                data.Themecomponentid = dataDto.ThemeComponentId;
                data.Modifieddate = DateTime.Now;
                
                // Mevcut durumda uygun repository metodu olmadığından en basit durumu gerçekleştirelim
                // İdeal olarak aşağıdaki ComponentRepository içine eklenmesi gereken metot:
                // public async Task<TAppSitecomponentdata> UpdateComponentDataAsync(TAppSitecomponentdata data)
                
                // Geçici olarak DTO'yu geri döndürüyoruz
                // NOT: Bu kısım ilgili repository metodu eklendikten sonra değiştirilmelidir
                var result = _mapper.Map<SiteComponentDataDto>(dataDto);
                
                // Bileşen adını doldur
                if (result.ComponentId.GetValueOrDefault() > 0)
                {
                    var component = await _componentRepository.GetComponentByIdAsync(result.ComponentId.GetValueOrDefault());
                    if (component != null)
                    {
                        result.ComponentName = component.Name;
                    }
                }
                
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Site bileşen verisi güncellenirken hata: {ex.Message}", ex);
            }
        }

        // Bileşen verisini sil
        public async Task DeleteComponentDataAsync(int dataId)
        {
            try
            {
                // Silmeden önce veriyi kontrol et
                var data = await _componentRepository.GetComponentDataByIdAsync(dataId);
                if (data == null)
                    throw new KeyNotFoundException($"Component data with ID {dataId} not found");
                
                // Mevcut durumda doğrudan DeleteAsync'i kullanıyoruz ancak
                // TAppSitecomponentdata için özel bir metod gerekebilir
                // Asıl Repository interface'inde şöyle bir metod olmalı:
                // Task DeleteComponentDataAsync(int dataId);
                
                await _componentRepository.DeleteAsync(dataId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Site bileşen verisi silinirken hata: {ex.Message}", ex);
            }
        }
    }
} 