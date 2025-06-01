using AutoMapper;
using Microsoft.EntityFrameworkCore;
using new_cms.Application.DTOs.MenuDTOs;
using new_cms.Application.Interfaces;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace new_cms.Application.Services
{
    /// Menü (TAppMenu) varlıkları ile ilgili işlemleri gerçekleştiren servis sınıfı.
    public class MenuService : IMenuService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IIdGeneratorService _idGenerator;

        /// MenuService sınıfının yeni bir örneğini başlatır.
        public MenuService(
            IUnitOfWork unitOfWork, 
            IMapper mapper,
            IIdGeneratorService idGenerator) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _idGenerator = idGenerator;
        }

        /// Belirtilen siteye ait en üst seviyedeki (ParentId'si null olan) aktif menüleri listeler.
        public async Task<IEnumerable<MenuListDto>> GetMenusBySiteIdAsync(int siteId)
        {
            if (siteId <= 0) {
                 throw new ArgumentException("Geçerli bir Site ID'si gereklidir.", nameof(siteId));
            }
            try
            {
                var topLevelMenus = await _unitOfWork.Repository<TAppMenu>().Query()
                    .Where(m => m.Siteid == siteId && m.Parentid == null && m.Isdeleted == 0) 
                    .OrderBy(m => m.Menuorder) 
                    .ToListAsync();
                
                return _mapper.Map<IEnumerable<MenuListDto>>(topLevelMenus);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Siteye ait menüler listelenirken hata oluştu (Site ID: {siteId}).", ex);
            }
        }

        /// Belirtilen menü ID'sinden başlayarak tüm alt menüleri hiyerarşik bir yapıda (MenuTreeDto) getirir.
        public async Task<MenuTreeDto?> GetMenuByIdAsync(int id)
        {
             if (id <= 0) {
                 throw new ArgumentException("Geçerli bir menü ID'si gereklidir.", nameof(id));
             }
             try
            {
                var rootMenuItem = await _unitOfWork.Repository<TAppMenu>().Query()
                                        .FirstOrDefaultAsync(m => m.Id == id && m.Isdeleted == 0); 

                if (rootMenuItem == null)
                    return null; 

                var allMenusForSite = await _unitOfWork.Repository<TAppMenu>().Query()
                    .Where(m => m.Siteid == rootMenuItem.Siteid && m.Isdeleted == 0) 
                    .OrderBy(m => m.Menuorder) 
                    .ToListAsync();
                
                var menuLookup = allMenusForSite.ToLookup(m => m.Parentid);

                return BuildMenuTreeRecursive(rootMenuItem, menuLookup);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Menü ağacı getirilirken hata oluştu (ID: {id}).", ex);
            }
        }
        
        /// Belirli bir site için tüm aktif menü öğelerini hiyerarşik bir ağaç yapısında getirir.
        public async Task<IEnumerable<MenuTreeDto>> GetMenuTreeBySiteIdAsync(int siteId)
        {
            if (siteId <= 0) {
                 throw new ArgumentException("Geçerli bir Site ID'si gereklidir.", nameof(siteId));
            }
            try
            {
                 var allMenusForSite = await _unitOfWork.Repository<TAppMenu>().Query()
                    .Where(m => m.Siteid == siteId && m.Isdeleted == 0) 
                    .OrderBy(m => m.Menuorder) 
                    .ToListAsync();

                // Ağacı oluşturmak için ParentId'ye göre grupla
                var menuLookup = allMenusForSite.ToLookup(m => m.Parentid);
                var rootMenus = menuLookup[null]; // ParentId'si null olanlar kök menülerdir

                // Her kök menü için ağacı oluştur
                var menuTree = new List<MenuTreeDto>();
                foreach (var rootMenu in rootMenus.OrderBy(m => m.Menuorder)) // Kök menüleri de sırala
                {
                    menuTree.Add(BuildMenuTreeRecursive(rootMenu, menuLookup));
                }
                return menuTree;
            }
             catch (Exception ex)
            {
                throw new InvalidOperationException($"Site için menü ağacı getirilirken hata oluştu (Site ID: {siteId}).", ex);
            }
        }

        /// Yeni bir menü öğesi oluşturur.
        public async Task<MenuDto> CreateMenuAsync(MenuDto menuDto)
        {
            if (menuDto == null)
                throw new ArgumentNullException(nameof(menuDto));
            if (menuDto.SiteId <= 0) 
                 throw new ArgumentException("Geçerli bir Site ID'si gereklidir.", nameof(menuDto.SiteId));
             if (string.IsNullOrWhiteSpace(menuDto.Name))
                 throw new ArgumentException("Menü adı boş olamaz.", nameof(menuDto.Name));

            try
            {
                var menu = _mapper.Map<TAppMenu>(menuDto);
                 
                menu.Id = await _idGenerator.GenerateNextIdAsync<TAppMenu>();
                menu.Isdeleted = 0; 
                menu.Createddate = DateTime.UtcNow; 
                // menu.Createduser = GetCurrentUserId(); // TODO: Aktif kullanıcı ID'si entegre edilmeli

                var createdMenu = await _unitOfWork.Repository<TAppMenu>().AddAsync(menu);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<MenuDto>(createdMenu);
            }
             catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Menü oluşturulurken veritabanı hatası: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
            catch (Exception ex)
            {
                 if (ex is ArgumentNullException || ex is ArgumentException) throw;
                throw new InvalidOperationException("Menü oluşturulurken beklenmedik bir hata oluştu.", ex);
            }
        }

        /// Mevcut bir menü öğesini günceller.
        public async Task<MenuDto> UpdateMenuAsync(MenuDto menuDto)
        {
             if (menuDto?.Id == null || menuDto.Id <= 0)
                throw new ArgumentNullException(nameof(menuDto), "Güncelleme için geçerli bir menü ID'si (DTO veya ID'si null) gereklidir.");
            if (string.IsNullOrWhiteSpace(menuDto.Name)){
                throw new ArgumentException("Menü adı boş olamaz.", nameof(menuDto.Name));
            }
             if (menuDto.SiteId <= 0) { 
                 throw new ArgumentException("Geçerli bir Site ID'si gereklidir.", nameof(menuDto.SiteId));
             }
            
            try
            {
                var existingMenu = await _unitOfWork.Repository<TAppMenu>().GetByIdAsync(menuDto.Id.Value); 
                
                if (existingMenu == null || existingMenu.Isdeleted == 1)
                    throw new KeyNotFoundException($"Güncellenecek menü bulunamadı veya silinmiş: ID {menuDto.Id.Value}");

                var originalIsDeleted = existingMenu.Isdeleted;
                var originalCreatedDate = existingMenu.Createddate;
                var originalCreatedUser = existingMenu.Createduser;
                var originalSiteId = existingMenu.Siteid; 
                var originalParentId = existingMenu.Parentid; 

                _mapper.Map(menuDto, existingMenu);

                existingMenu.Isdeleted = originalIsDeleted;
                existingMenu.Createddate = originalCreatedDate;
                existingMenu.Createduser = originalCreatedUser;
                existingMenu.Siteid = originalSiteId; 
                existingMenu.Parentid = originalParentId; 

                existingMenu.Modifieddate = DateTime.UtcNow;
                // existingMenu.Modifieduser = GetCurrentUserId(); // TODO: Aktif kullanıcı ID'si eklenmeli

                await _unitOfWork.Repository<TAppMenu>().UpdateAsync(existingMenu);
                
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<MenuDto>(existingMenu);

            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Menü güncellenirken veritabanı hatası oluştu (ID: {menuDto.Id.Value}): {ex.InnerException?.Message ?? ex.Message}", ex);
            }
             catch (Exception ex)
            {
                 if (ex is KeyNotFoundException || ex is ArgumentNullException || ex is ArgumentException || ex is InvalidOperationException) throw;
                throw new InvalidOperationException($"Menü güncellenirken beklenmedik bir hata oluştu (ID: {menuDto.Id.Value}).", ex);
            }
        }

        /// Belirtilen ID'ye sahip menü öğesini ve tüm alt öğelerini (recursive) pasif hale getirir (soft delete).
        public async Task DeleteMenuAsync(int id)
        {
            if (id <= 0) {
                 throw new ArgumentException("Geçerli bir menü ID'si gereklidir.", nameof(id));
            }
            try
            {
                var menuToDelete = await _unitOfWork.Repository<TAppMenu>().GetByIdAsync(id);
                if (menuToDelete == null || menuToDelete.Isdeleted == 1)
                {
                    throw new KeyNotFoundException($"Silinecek menü bulunamadı veya zaten silinmiş: ID {id}"); 
                }

                var itemsToSoftDelete = new List<TAppMenu>();
                await CollectDescendantsForSoftDeleteAsync(menuToDelete, itemsToSoftDelete);

                await _unitOfWork.Repository<TAppMenu>().SoftDeleteRangeAsync(itemsToSoftDelete);

                DateTime utcNow = DateTime.UtcNow;
                // int? currentUserId = GetCurrentUserId(); // TODO: Aktif kullanıcı ID'si eklenmeli

                foreach(var item in itemsToSoftDelete)
                {
                    item.Modifieddate = utcNow;
                    // item.Modifieduser = currentUserId;
                }

                // Değişiklikleri veritabanına kaydet
                await _unitOfWork.CompleteAsync();
            }
             catch (Exception ex)
            {
                 if (ex is KeyNotFoundException || ex is ArgumentException) throw;
                throw new InvalidOperationException($"Menü ve alt öğeleri silinirken bir hata oluştu (ID: {id}).", ex);
            }
        }
        
        // --- Yardımcı Metotlar --- 

        /// Verilen menü öğesinin tüm alt öğelerini (recursive) bularak silinecekler listesine ekler. 
        private async Task CollectDescendantsForSoftDeleteAsync(TAppMenu currentMenu, List<TAppMenu> itemsToSoftDelete)
        {
            if (!itemsToSoftDelete.Contains(currentMenu)) {
                 itemsToSoftDelete.Add(currentMenu);
            }

            // Mevcut menünün aktif alt öğelerini çek
            var children = await _unitOfWork.Repository<TAppMenu>().Query()
                                .Where(m => m.Parentid == currentMenu.Id && m.Isdeleted == 0) // Sadece aktif alt öğeler
                                .ToListAsync();

            // Her bir alt öğe için recursive olarak devam et
            foreach (var child in children)
            {
                await CollectDescendantsForSoftDeleteAsync(child, itemsToSoftDelete); 
            }
        }

        /// Verilen menü öğesinden başlayarak hiyerarşik MenuTreeDto yapısını recursive olarak oluşturur.
        /// Bu metod veritabanı erişimi yapmaz, kendisine verilen Lookup listesi üzerinden çalışır.
        private MenuTreeDto BuildMenuTreeRecursive(TAppMenu currentMenu, ILookup<int?, TAppMenu> menuLookup)
        {
            var menuTreeItem = _mapper.Map<MenuTreeDto>(currentMenu);
            
            var children = menuLookup[currentMenu.Id].OrderBy(m => m.Menuorder);
            if (children.Any())
            {
                menuTreeItem.Children = new List<MenuTreeDto>();
                foreach (var child in children)
                {
                    menuTreeItem.Children.Add(BuildMenuTreeRecursive(child, menuLookup));
                }
            }
            return menuTreeItem;
        }

        /// Sistemdeki tüm menüleri listeler.
        public async Task<IEnumerable<MenuListDto>> GetAllMenusAsync()
        {
            try
            {
                var allMenus = await _unitOfWork.Repository<TAppMenu>().Query()
                    .OrderBy(m => m.Siteid)
                    .ThenBy(m => m.Menuorder)
                    .ToListAsync();
                
                return _mapper.Map<IEnumerable<MenuListDto>>(allMenus);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Tüm menüler listelenirken bir hata oluştu.", ex);
            }
        }

        /// Belirtilen üst menüye ait alt menüleri listeler.
        public async Task<IEnumerable<MenuListDto>> GetMenusByParentIdAsync(int parentId)
        {
            if (parentId <= 0)
            {
                throw new ArgumentException("Geçerli bir üst menü ID'si gereklidir.", nameof(parentId));
            }

            try
            {
                var childMenus = await _unitOfWork.Repository<TAppMenu>().Query()
                    .Where(m => m.Parentid == parentId && m.Isdeleted == 0)
                    .OrderBy(m => m.Menuorder)
                    .ToListAsync();
                
                return _mapper.Map<IEnumerable<MenuListDto>>(childMenus);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Alt menüler listelenirken hata oluştu (Üst menü ID: {parentId}).", ex);
            }
        }
    }
} 