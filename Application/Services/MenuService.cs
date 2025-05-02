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
    /// Menü yönetimi (TAppMenu) ile ilgili işlemleri gerçekleştiren servis sınıfı.
    public class MenuService : IMenuService
    {
        private readonly IRepository<TAppMenu> _menuRepository;
        private readonly IMapper _mapper;

        public MenuService(IRepository<TAppMenu> menuRepository, IMapper mapper)
        {
            _menuRepository = menuRepository;
            _mapper = mapper;
        }

        /// Belirtilen siteye ait en üst seviyedeki aktif menüleri listeler.
        public async Task<IEnumerable<MenuListDto>> GetMenusBySiteIdAsync(int siteId)
        {
            try
            {
                var topLevelMenus = await _menuRepository.Query()
                    .Where(m => m.Siteid == siteId && m.Parentid == null && m.Isdeleted == 0)
                    .OrderBy(m => m.Menuorder) // Menuorder'a göre sırala
                    .ToListAsync();
                
                return _mapper.Map<IEnumerable<MenuListDto>>(topLevelMenus);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Siteye ait menüler listelenirken hata oluştu (Site ID: {siteId}).", ex);
            }
        }

        /// Belirtilen menü ID'sinden başlayarak tüm alt menüleri hiyerarşik olarak getirir.
        public async Task<MenuTreeDto?> GetMenuByIdAsync(int id)
        {
             try
            {
                var rootMenuItem = await _menuRepository.Query()
                                        .FirstOrDefaultAsync(m => m.Id == id && m.Isdeleted == 0);

                if (rootMenuItem == null)
                    return null;

                // Bu menünün ait olduğu sitedeki tüm menüleri çekelim
                var allMenusForSite = await _menuRepository.Query()
                    .Where(m => m.Siteid == rootMenuItem.Siteid && m.Isdeleted == 0)
                    .OrderBy(m => m.Menuorder)
                    .ToListAsync();
                
                // Ağacı oluşturmak için tüm menüleri lookup'a çevirelim
                var menuLookup = allMenusForSite.ToLookup(m => m.Parentid);

                // Recursive olarak ağacı oluşturalım
                return BuildMenuTreeRecursive(rootMenuItem, menuLookup);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Menü ağacı getirilirken hata oluştu (ID: {id}).", ex);
            }
        }
        
        /// Belirli bir site için tüm menü öğelerini hiyerarşik olarak getirir.
        public async Task<IEnumerable<MenuTreeDto>> GetMenuTreeBySiteIdAsync(int siteId)
        {
            try
            {
                 // Siteye ait tüm aktif menüleri çekelim
                 var allMenusForSite = await _menuRepository.Query()
                    .Where(m => m.Siteid == siteId && m.Isdeleted == 0)
                    .OrderBy(m => m.Menuorder)
                    .ToListAsync();

                // Ağacı oluşturmak için lookup ve kökleri bulalım
                var menuLookup = allMenusForSite.ToLookup(m => m.Parentid);
                var rootMenus = menuLookup[null]; // ParentId'si null olanlar kök menülerdir

                // Her kök menü için ağacı oluşturalım
                var menuTree = new List<MenuTreeDto>();
                foreach (var rootMenu in rootMenus)
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
                menu.Isdeleted = 0;
                menu.Createddate = DateTime.UtcNow;
                // menu.Createduser = GetCurrentUserId(); 

                // Varsayılan Status (Aktif) ataması yapılabilir, DTO'da yoksa.
                menu.Status ??= 1;

                var createdMenu = await _menuRepository.AddAsync(menu);
                return _mapper.Map<MenuDto>(createdMenu);
            }
             catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Menü oluşturulurken veritabanı hatası oluştu.", ex);
            }
            catch (Exception ex)
            {
                 if (ex is ArgumentException) throw;
                throw new InvalidOperationException("Menü oluşturulurken beklenmedik bir hata oluştu.", ex);
            }
        }

        /// Mevcut bir menü öğesini günceller.
        public async Task<MenuDto> UpdateMenuAsync(MenuDto menuDto)
        {
             if (menuDto?.Id == null || menuDto.Id <= 0)
                throw new ArgumentNullException(nameof(menuDto), "Güncelleme için geçerli bir menü ID'si gereklidir.");
            
            try
            {
                var existingMenu = await _menuRepository.GetByIdAsync(menuDto.Id.Value);
                 if (existingMenu == null || existingMenu.Isdeleted == 1)
                    throw new KeyNotFoundException($"Güncellenecek menü bulunamadı veya silinmiş: ID {menuDto.Id.Value}");

                var originalIsDeleted = existingMenu.Isdeleted;
                var originalCreatedDate = existingMenu.Createddate;
                var originalCreatedUser = existingMenu.Createduser;
                var originalSiteId = existingMenu.Siteid; 

                _mapper.Map(menuDto, existingMenu);

                existingMenu.Isdeleted = originalIsDeleted;
                existingMenu.Createddate = originalCreatedDate;
                existingMenu.Createduser = originalCreatedUser;
                existingMenu.Siteid = originalSiteId; 

                existingMenu.Modifieddate = DateTime.UtcNow;
                 // existingMenu.Modifieduser = GetCurrentUserId(); 

                await _menuRepository.UpdateAsync(existingMenu);
                return _mapper.Map<MenuDto>(existingMenu);

            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Menü güncellenirken veritabanı hatası oluştu (ID: {menuDto.Id.Value}).", ex);
            }
             catch (Exception ex)
            {
                 if (ex is KeyNotFoundException || ex is InvalidOperationException) throw;
                throw new InvalidOperationException($"Menü güncellenirken beklenmedik bir hata oluştu (ID: {menuDto.Id.Value}).", ex);
            }
        }

        /// Belirtilen ID'ye sahip menü öğesini ve tüm alt öğelerini pasif hale getirir (soft delete).
        public async Task DeleteMenuAsync(int id)
        {
            try
            {
                // Silinecek ana menü öğesini bulma
                var menuToDelete = await _menuRepository.GetByIdAsync(id);
                if (menuToDelete == null || menuToDelete.Isdeleted == 1)
                {
                    throw new KeyNotFoundException($"Silinecek menü bulunamadı veya zaten silinmiş: ID {id}"); 
                }

                // Ana menünün ve tüm alt öğelerinin listesini oluştur
                var itemsToDelete = new List<TAppMenu> { menuToDelete };
                var descendants = await GetAllDescendantsAsync(id, menuToDelete.Siteid); 
                itemsToDelete.AddRange(descendants);

                // Toplu soft delete yap
                await _menuRepository.SoftDeleteRangeAsync(itemsToDelete);
            }
             catch (Exception ex)
            {
                throw new InvalidOperationException($"Menü ve alt öğeleri silinirken bir hata oluştu (ID: {id}).", ex);
            }
        }
        
        // --- Yardımcı Metotlar ---

        /// Verilen menü öğesi ID'sinin tüm alt öğelerini veritabanından bulur.
        private async Task<List<TAppMenu>> GetAllDescendantsAsync(int parentId, int? siteId)
        {
            var descendants = new List<TAppMenu>();
            
            var query = _menuRepository.Query()
                                .Where(m => m.Parentid == parentId && m.Isdeleted == 0);
            if (siteId.HasValue)
            {
                query = query.Where(m => m.Siteid == siteId.Value);
            }

            var children = await query.ToListAsync();

            foreach (var child in children)
            {
                descendants.Add(child);
                descendants.AddRange(await GetAllDescendantsAsync(child.Id, siteId)); 
            }
            return descendants;
        }

        /// Verilen menü öğesinden başlayarak hiyerarşik MenuTreeDto yapısını recursive olarak oluşturur.
        private MenuTreeDto BuildMenuTreeRecursive(TAppMenu currentMenu, ILookup<int?, TAppMenu> menuLookup)
        {
            var menuTreeItem = _mapper.Map<MenuTreeDto>(currentMenu);
            
            var children = menuLookup[currentMenu.Id]; 
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
    }
} 