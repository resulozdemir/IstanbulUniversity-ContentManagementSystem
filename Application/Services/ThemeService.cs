using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using new_cms.Application.DTOs.ThemeDTOs;
using new_cms.Application.DTOs.ComponentDTOs;
using new_cms.Application.Interfaces;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;

namespace new_cms.Application.Services
{
    public class ThemeService : IThemeService
    {
        private readonly IThemeRepository _themeRepository;
        private readonly IComponentRepository _componentRepository;
        private readonly IMapper _mapper;

        public ThemeService(
            IThemeRepository themeRepository, 
            IComponentRepository componentRepository, 
            IMapper mapper)
        {
            _themeRepository = themeRepository ?? throw new ArgumentNullException(nameof(themeRepository));
            _componentRepository = componentRepository ?? throw new ArgumentNullException(nameof(componentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // Tüm temaları listele
        public async Task<IEnumerable<ThemeDto>> GetAllThemesAsync()
        {
            // Tüm temaları getir
            var themes = await _themeRepository.GetAllThemesAsync();
            
            // DTO'ya dönüştür
            return _mapper.Map<IEnumerable<ThemeDto>>(themes);
        }

        // Tema detayını getir
        public async Task<ThemeDto?> GetThemeByIdAsync(int id)
        {
            // Tema bilgisini getir
            var theme = await _themeRepository.GetThemeByIdAsync(id);
            if (theme == null)
                return null;
                
            // DTO'ya dönüştür
            return _mapper.Map<ThemeDto>(theme);
        }

        // Yeni tema oluştur
        public async Task<ThemeDto> CreateThemeAsync(ThemeDto themeDto)
        {
            // DTO'yu entity'e dönüştür
            var theme = _mapper.Map<TAppTheme>(themeDto);
            
            // Repository üzerinden kaydet
            var createdTheme = await _themeRepository.AddAsync(theme);
            
            // Kaydedilen entity'yi DTO'ya dönüştür ve geri döndür
            return _mapper.Map<ThemeDto>(createdTheme);
        }

        // Tema bilgilerini güncelle
        public async Task<ThemeDto> UpdateThemeAsync(ThemeDto themeDto)
        {
            // Güncellenecek tema ID kontrol et
            if (!themeDto.Id.HasValue)
                throw new ArgumentException("Theme ID is required for update operation");
            
            // Mevcut tema verilerini getir
            var existingTheme = await _themeRepository.GetByIdAsync(themeDto.Id.Value);
            if (existingTheme == null)
                throw new KeyNotFoundException($"Theme with ID {themeDto.Id.Value} not found");
            
            // DTO'dan entity'ye dönüştür
            var theme = _mapper.Map<ThemeDto, TAppTheme>(themeDto, existingTheme);
            
            // Repository üzerinden güncelle
            var updatedTheme = await _themeRepository.UpdateAsync(theme);
            
            // Güncellenen entity'yi DTO'ya dönüştür ve geri döndür
            return _mapper.Map<ThemeDto>(updatedTheme);
        }

        // Tema sil (soft delete)
        public async Task DeleteThemeAsync(int id)
        {
            // Temanın kullanımda olup olmadığını kontrol et
            if (await IsThemeInUseAsync(id))
                throw new InvalidOperationException($"Theme with ID {id} is in use and cannot be deleted");
                
            // Soft delete kullanarak temayı sil
            await _themeRepository.SoftDeleteAsync(id);
        }

        // Tema bileşenlerini listele
        public async Task<IEnumerable<ThemeComponentDto>> GetThemeComponentsAsync(int themeId)
        {
            // Tema bileşenlerini getir
            var components = await _themeRepository.GetThemeComponentsAsync(themeId);
            if (components == null)
                return new List<ThemeComponentDto>();
                
            // DTO'ya dönüştür
            var componentDtos = _mapper.Map<IEnumerable<ThemeComponentDto>>(components);
            
            // Bileşen adlarını doldur
            foreach (var component in componentDtos)
            {
                if (component.ComponentId > 0) // Nullable kontrolü yerine değer kontrolü yapıyoruz
                {
                    var componentEntity = await _componentRepository.GetComponentByIdAsync(component.ComponentId);
                    if (componentEntity != null)
                    {
                        component.ComponentName = componentEntity.Name;
                    }
                }
            }
            
            return componentDtos;
        }

        // Temaya bileşen ekle
        public async Task<ThemeComponentDto> AddComponentToThemeAsync(ThemeComponentDto componentDto)
        {
            try
            {
                // DTO'yu entity'e dönüştür
                var component = _mapper.Map<TAppThemecomponent>(componentDto);
                
                // İlişkileri doğru şekilde ayarla
                component.Themeid = componentDto.ThemeId;
                component.Componentid = componentDto.ComponentId;
                
                // Mevcut durumda uygun repository metodu olmadığından en basit durumu gerçekleştirelim
                // İdeal olarak aşağıdaki ThemeRepository içine eklenmesi gereken metot:
                // public async Task<TAppThemecomponent> AddThemeComponentAsync(TAppThemecomponent component)
                
                // Geçici olarak componentDto'yu geri döndürüyoruz
                // NOT: Bu kısım, IThemeRepository'ye AddThemeComponentAsync metodu eklendikten sonra değiştirilmelidir
                var result = _mapper.Map<ThemeComponentDto>(componentDto);
                
                // Bileşen adını doldur
                if (result.ComponentId > 0)
                {
                    var componentEntity = await _componentRepository.GetComponentByIdAsync(result.ComponentId);
                    if (componentEntity != null)
                    {
                        result.ComponentName = componentEntity.Name;
                    }
                }
                
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Tema bileşeni eklenirken hata: {ex.Message}", ex);
            }
        }

        // Tema bileşenini güncelle
        public async Task<ThemeComponentDto> UpdateThemeComponentAsync(ThemeComponentDto componentDto)
        {
            try
            {
                // Güncellenecek bileşen ID kontrol et
                if (!componentDto.Id.HasValue)
                    throw new ArgumentException("Component ID is required for update operation");
                
                // Mevcut bileşen verilerini getir
                var existingComponent = await _themeRepository.GetThemeComponentByIdAsync(componentDto.Id.Value);
                if (existingComponent == null)
                    throw new KeyNotFoundException($"Theme component with ID {componentDto.Id.Value} not found");
                
                // DTO'dan entity'ye dönüştür, mevcut verileri koru
                var component = _mapper.Map<ThemeComponentDto, TAppThemecomponent>(componentDto, existingComponent);
                
                // İlişkileri doğru şekilde ayarla
                component.Themeid = componentDto.ThemeId; 
                component.Componentid = componentDto.ComponentId;
                
                var result = _mapper.Map<ThemeComponentDto>(componentDto);
                
                // Bileşen adını doldur
                if (result.ComponentId > 0)
                {
                    var componentEntity = await _componentRepository.GetComponentByIdAsync(result.ComponentId);
                    if (componentEntity != null)
                    {
                        result.ComponentName = componentEntity.Name;
                    }
                }
                
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Tema bileşeni güncellenirken hata: {ex.Message}", ex);
            }
        }

        // Tema bileşenini kaldır
        public async Task RemoveComponentFromThemeAsync(int componentId)
        {
            // Silmeden önce bileşenin varlığını kontrol et
            var component = await _themeRepository.GetThemeComponentByIdAsync(componentId);
            if (component == null)
                throw new KeyNotFoundException($"Theme component with ID {componentId} not found");
                
            // Tema bileşenini sil - DeleteAsync repository metodunu kullanıyoruz
            await _themeRepository.DeleteAsync(componentId);
        }

        // Temaya eklenebilecek uygun bileşenleri listele
        public async Task<IEnumerable<ComponentDto>> GetAvailableComponentsForThemeAsync(int themeId)
        {
            // Temaya eklenebilecek bileşenleri getir
            var components = await _themeRepository.GetAvailableComponentsForThemeAsync(themeId);
            
            // DTO'ya dönüştür
            return _mapper.Map<IEnumerable<ComponentDto>>(components);
        }

        // Temanın kullanımda olup olmadığını kontrol et
        public async Task<bool> IsThemeInUseAsync(int id)
        {
            return await _themeRepository.IsThemeInUseAsync(id);
        }

        // Tema bileşen detaylarını getir
        public async Task<ThemeComponentDto?> GetThemeComponentByIdAsync(int componentId)
        {
            // Tema bileşenini getir
            var component = await _themeRepository.GetThemeComponentByIdAsync(componentId);
            if (component == null)
                return null;
                
            // DTO'ya dönüştür
            var componentDto = _mapper.Map<ThemeComponentDto>(component);
            
            // Bileşen adını doldur
            if (componentDto.ComponentId > 0) // Nullable kontrolü yerine değer kontrolü yapıyoruz
            {
                var componentEntity = await _componentRepository.GetComponentByIdAsync(componentDto.ComponentId);
                if (componentEntity != null)
                {
                    componentDto.ComponentName = componentEntity.Name;
                }
            }
            
            return componentDto;
        }
    }
} 