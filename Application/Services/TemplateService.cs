using AutoMapper;
using Microsoft.EntityFrameworkCore;
using new_cms.Application.DTOs.SiteDTOs;
using new_cms.Application.DTOs.TemplateDTOs;
using new_cms.Application.Interfaces;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace new_cms.Application.Services
{
    // Site şablonları (template) ile ilgili işlemleri gerçekleştiren servis sınıfı.
    public class TemplateService : ITemplateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TemplateService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Yeni bir site şablonu oluşturur.
        public async Task<TemplateDto> CreateTemplateAsync(TemplateDto templateDto)
        {
            var templateEntity = _mapper.Map<TAppSite>(templateDto);

            templateEntity.Istemplate = 1;  
            templateEntity.Isdeleted = 0;
            templateEntity.Createddate = DateTime.UtcNow;
            // templateEntity.Createduser = GetCurrentUserId(); 

            var themeExists = await _unitOfWork.Repository<TAppTheme>().Query()
                                    .AnyAsync(t => t.Id == templateDto.ThemeId && t.Isdeleted == 0);
            if (!themeExists)
            {
                throw new KeyNotFoundException($"Belirtilen tema ID ({templateDto.ThemeId}) bulunamadı veya aktif değil.");
            }

            var createdTemplate = await _unitOfWork.Repository<TAppSite>().AddAsync(templateEntity);
            await _unitOfWork.CompleteAsync();

            var resultDto = _mapper.Map<TemplateDto>(createdTemplate);
            
            if (createdTemplate.Themeid > 0) {
                var theme = await _unitOfWork.Repository<TAppTheme>().Query()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == createdTemplate.Themeid && t.Isdeleted == 0);
                if (theme != null) resultDto.ThemeName = theme.Name;
            }
            return resultDto;
        }

        // Sistemdeki tüm aktif site şablonlarını listeler.
        public async Task<IEnumerable<TemplateDto>> GetAllTemplatesAsync()
        {
            var templates = await _unitOfWork.Repository<TAppSite>().Query()
                .Where(s => s.Istemplate == 1 && s.Isdeleted == 0)
                .OrderBy(s => s.Name) 
                .AsNoTracking()
                .ToListAsync();
            
            var templateDtos = _mapper.Map<IEnumerable<TemplateDto>>(templates);

            return templateDtos;
        }

        // Belirtilen ID'ye sahip site şablonunun detaylarını getirir.
        public async Task<TemplateDto?> GetTemplateByIdAsync(int templateId)
        {
            var template = await _unitOfWork.Repository<TAppSite>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == templateId && s.Istemplate == 1 && s.Isdeleted == 0);
            
            if (template == null) return null;

            var templateDto = _mapper.Map<TemplateDto>(template);

            return templateDto;
        }

        // Mevcut bir site şablonunu günceller.
        public async Task<TemplateDto> UpdateTemplateAsync(int templateId, TemplateDto templateDto)
        {
            var existingTemplate = await _unitOfWork.Repository<TAppSite>().Query()
                .Where(s => s.Id == templateId && s.Istemplate == 1 && s.Isdeleted == 0)
                .FirstOrDefaultAsync();

            if (existingTemplate == null)
            {
                throw new KeyNotFoundException($"Güncellenecek şablon (ID: {templateId}) bulunamadı, şablon değil veya silinmiş.");
            }

            var themeExists = await _unitOfWork.Repository<TAppTheme>().Query()
                                    .AnyAsync(t => t.Id == templateDto.ThemeId && t.Isdeleted == 0);
            if (!themeExists)
            {
                throw new KeyNotFoundException($"Belirtilen tema ID ({templateDto.ThemeId}) bulunamadı veya aktif değil.");
            }

            _mapper.Map(templateDto, existingTemplate); 
            
            existingTemplate.Istemplate = 1;  
            existingTemplate.Modifieddate = DateTime.UtcNow;
            // existingTemplate.Modifieduser = GetCurrentUserId();

            await _unitOfWork.Repository<TAppSite>().UpdateAsync(existingTemplate);
            await _unitOfWork.CompleteAsync();

            var resultDto = _mapper.Map<TemplateDto>(existingTemplate);

            return resultDto;
        }

        // Belirtilen ID'ye sahip site şablonunu pasif hale getirir (soft delete).
        public async Task DeleteTemplateAsync(int templateId)
        {
            var templateToDelete = await _unitOfWork.Repository<TAppSite>().Query()
                .Where(s => s.Id == templateId && s.Istemplate == 1)
                .FirstOrDefaultAsync();

            if (templateToDelete == null) 
            {
                throw new KeyNotFoundException($"Silinecek şablon bulunamadı: ID {templateId}");
            }
            if (templateToDelete.Isdeleted == 1)
            {
                throw new InvalidOperationException($"Şablon zaten silinmiş: ID {templateId}");
            }

            var sitesUsingTemplate = await _unitOfWork.Repository<TAppSite>().Query()
                .AnyAsync(s => s.Templateid == templateId && s.Isdeleted == 0 && s.Istemplate == 0);

            if (sitesUsingTemplate)
            {
                throw new InvalidOperationException($"Bu şablon (ID: {templateId}) bir veya daha fazla aktif site tarafından kullanıldığı için silinemez.");
            }
            
            await _unitOfWork.Repository<TAppSite>().SoftDeleteAsync(templateId);
            await _unitOfWork.CompleteAsync();
        }

        // Belirli bir şablonu kullanan tüm aktif siteleri listeler.
        public async Task<IEnumerable<SiteListDto>> GetSitesByTemplateAsync(int templateId)
        {
            var templateExists = await _unitOfWork.Repository<TAppSite>().Query()
                .AnyAsync(s => s.Id == templateId && s.Istemplate == 1 && s.Isdeleted == 0);

            if (!templateExists)
            {
                return Enumerable.Empty<SiteListDto>();
            }

            var sites = await _unitOfWork.Repository<TAppSite>().Query()
                .Where(s => s.Templateid == templateId && s.Isdeleted == 0 && s.Istemplate == 0) 
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<SiteListDto>>(sites);
        }

        // Bir kaynak şablonun içeriğini (sayfalar, bileşenler vb.) hedef bir siteye kopyalar.
        public async Task CopyTemplateContentToSiteAsync(int sourceTemplateId, int targetSiteId)
        {
            var sourceTemplateExists = await _unitOfWork.Repository<TAppSite>().Query()
                .AnyAsync(s => s.Id == sourceTemplateId && s.Istemplate == 1 && s.Isdeleted == 0);
            if (!sourceTemplateExists)
            {
                throw new KeyNotFoundException($"Kaynak şablon (ID: {sourceTemplateId}) bulunamadı veya aktif değil.");
            }

            var targetSiteExists = await _unitOfWork.Repository<TAppSite>().Query()
                .AnyAsync(s => s.Id == targetSiteId && s.Istemplate == 0 && s.Isdeleted == 0);
            if (!targetSiteExists)
            {
                throw new KeyNotFoundException($"Hedef site (ID: {targetSiteId}) bulunamadı veya aktif değil.");
            }

            // Sayfa kopyalama (TAppSitepage).
            var templatePages = await _unitOfWork.Repository<TAppSitepage>().Query()
                .AsNoTracking()
                .Where(p => p.Siteid == sourceTemplateId && p.Isdeleted == 0)
                .ToListAsync();

            foreach (var pageToCopy in templatePages)
            {
                var newPage = _mapper.Map<TAppSitepage>(pageToCopy);
                newPage.Id = 0;
                newPage.Siteid = targetSiteId;
                newPage.Createddate = DateTime.UtcNow;
                // newPage.Createduser = GetCurrentUserId(); 
                await _unitOfWork.Repository<TAppSitepage>().AddAsync(newPage);
            }
            
            // Component kopyalama (TAppSitecomponentdata).
            var templateComponents = await _unitOfWork.Repository<TAppSitecomponentdata>().Query()
                .AsNoTracking()
                .Where(c => c.Siteid == sourceTemplateId && c.Isdeleted == 0)
                .ToListAsync();

            foreach (var componentToCopy in templateComponents)
            {
                var newComponent = _mapper.Map<TAppSitecomponentdata>(componentToCopy);
                newComponent.Id = 0; 
                newComponent.Siteid = targetSiteId; 
                newComponent.Createddate = DateTime.UtcNow;
                await _unitOfWork.Repository<TAppSitecomponentdata>().AddAsync(newComponent);
            }

            // Menüleri kopyala (TAppMenu)
            var templateMenus = await _unitOfWork.Repository<TAppMenu>().Query()
                .AsNoTracking()
                .Where(m => m.Siteid == sourceTemplateId && m.Isdeleted == 0)
                .ToListAsync();

            // İlk kopyalama aşaması - Tüm menüleri kopyala
            var oldToNewMenuIds = new Dictionary<int, int>(); // Eski-Yeni ID eşleştirmeleri
            foreach (var menuToCopy in templateMenus)
            {
                var newMenu = _mapper.Map<TAppMenu>(menuToCopy);
                newMenu.Id = 0;
                newMenu.Siteid = targetSiteId;
                newMenu.Createddate = DateTime.UtcNow;
                
                var addedMenu = await _unitOfWork.Repository<TAppMenu>().AddAsync(newMenu);
                await _unitOfWork.CompleteAsync(); // ID'leri almak için kaydet
                
                // Eski-Yeni ID eşleştirmesini kaydet
                oldToNewMenuIds.Add(menuToCopy.Id, addedMenu.Id);
            }
            
            // İkinci aşama - Üst menü (parent) referanslarını güncelle
            foreach (var menuId in oldToNewMenuIds.Values)
            {
                var menu = await _unitOfWork.Repository<TAppMenu>().GetByIdAsync(menuId);
                if (menu != null && menu.Parentid.HasValue && menu.Parentid.Value > 0)
                {
                    // Eski parent ID'si yeni ID ile değiştir
                    if (oldToNewMenuIds.TryGetValue(menu.Parentid.Value, out var newParentId))
                    {
                        menu.Parentid = newParentId;
                        await _unitOfWork.Repository<TAppMenu>().UpdateAsync(menu);
                    }
                }
            }

            await _unitOfWork.CompleteAsync();
        }
    }
} 