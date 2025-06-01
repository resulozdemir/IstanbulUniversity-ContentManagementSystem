using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using new_cms.Application.DTOs.SiteDTOs;
using new_cms.Application.Interfaces;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;

namespace new_cms.Application.Services
{ 
    /// Site alan adlarını yönetmek için servis sınıfı. 
    public class SiteDomainService : ISiteDomainService
    { 
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SiteDomainService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// Belirtilen site ID'sine ait tüm aktif alan adlarını listeler.  
        public async Task<IEnumerable<SiteDomainDto>> GetDomainsBySiteIdAsync(int siteId)
        { 
            if (siteId <= 0) {
                 throw new ArgumentException("Geçerli bir Site ID'si gereklidir.", nameof(siteId));
            }

            try { 
                var domains = await _unitOfWork.Repository<TAppSitedomain>().Query()
                    .Where(d => d.Siteid == siteId && d.Isdeleted == 0)  
                    .ToListAsync();
 
                return _mapper.Map<IEnumerable<SiteDomainDto>>(domains);
            } catch(Exception ex) { 
                throw new InvalidOperationException($"Site ID '{siteId}' için alan adları listelenirken hata: {ex.Message}", ex);
            }
        }
 
        /// Yeni bir site alan adı (domain) oluşturur.
        public async Task<SiteDomainDto> CreateDomainAsync(SiteDomainDto domainDto)
        {
            if (domainDto.SiteId <= 0) 
                 throw new ArgumentException("Geçerli bir Site ID belirtilmelidir.", nameof(domainDto.SiteId));
            if (string.IsNullOrWhiteSpace(domainDto.Language))
                throw new ArgumentException("Dil alanı boş olamaz.", nameof(domainDto.Language)); 
            if (string.IsNullOrWhiteSpace(domainDto.Key))
                throw new ArgumentException("Key alanı boş olamaz.", nameof(domainDto.Key));

            // Domain boş değilse benzersizlik kontrolü yap
            if (!string.IsNullOrWhiteSpace(domainDto.Domain) && !await IsDomainUniqueAsync(domainDto.Domain))
                throw new InvalidOperationException($"'{domainDto.Domain}' alan adı zaten kullanılıyor.");

            try
            { 
                var domain = _mapper.Map<TAppSitedomain>(domainDto);
 
                domain.Createddate = DateTime.UtcNow;
                domain.Isdeleted = 0; 
                // TODO: domain.Createduser = GetCurrentUserId(); 
 
                var createdDomain = await _unitOfWork.Repository<TAppSitedomain>().AddAsync(domain);
                 
                return _mapper.Map<SiteDomainDto>(createdDomain); 
            }
            catch (DbUpdateException ex) 
            { 
                throw new InvalidOperationException($"Alan adı veritabanına eklenirken hata oluştu: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
            catch (Exception ex)
            { 
                 if (ex is ArgumentException || ex is InvalidOperationException) throw;  
                throw new InvalidOperationException($"Alan adı eklenirken beklenmedik bir hata oluştu: {ex.Message}", ex);
            }
        }
 
        /// Mevcut bir alan adının bilgilerini günceller.
        public async Task<SiteDomainDto> UpdateDomainAsync(SiteDomainDto domainDto)
        { 
            if (domainDto.Id == null || domainDto.Id <= 0)
                throw new ArgumentException("Güncelleme için geçerli bir alan adı ID'si gereklidir.", nameof(domainDto.Id));
            if (string.IsNullOrWhiteSpace(domainDto.Domain))  
                throw new ArgumentException("Domain alanı boş olamaz.", nameof(domainDto.Domain));
            if (domainDto.SiteId <= 0)
                 throw new ArgumentException("Geçerli bir Site ID belirtilmelidir.", nameof(domainDto.SiteId));
             if (string.IsNullOrWhiteSpace(domainDto.Language))
                 throw new ArgumentException("Dil alanı boş olamaz.", nameof(domainDto.Language));
            if (string.IsNullOrWhiteSpace(domainDto.Key))
                throw new ArgumentException("Key alanı boş olamaz.", nameof(domainDto.Key));

            try
            { 
                var existingDomain = await _unitOfWork.Repository<TAppSitedomain>().GetByIdAsync(domainDto.Id.Value); 
                if (existingDomain == null || existingDomain.Isdeleted == 1)
                    throw new KeyNotFoundException($"ID: {domainDto.Id} olan alan adı bulunamadı veya silinmiş.");
 
                 if (existingDomain.Domain != domainDto.Domain) { 
                     throw new InvalidOperationException("Domain adı değiştirilemez."); 
                 }

 
                var originalDomain = existingDomain.Domain;  
                var originalSiteId = existingDomain.Siteid;  
                var originalIsDeleted = existingDomain.Isdeleted; 
                var originalCreatedDate = existingDomain.Createddate;  
                var originalCreatedUser = existingDomain.Createduser;  

                // DTO'daki verileri mevcut entity üzerine haritala
                _mapper.Map(domainDto, existingDomain);

                 
                existingDomain.Domain = originalDomain;  
                existingDomain.Siteid = originalSiteId;  
                existingDomain.Isdeleted = originalIsDeleted;
                existingDomain.Createddate = originalCreatedDate;
                existingDomain.Createduser = originalCreatedUser;
 
                existingDomain.Modifieddate = DateTime.UtcNow;
                //existingDomain.Modifieduser = GetCurrentUserId(); // TODO: Aktif kullanıcı ID'si eklenmeli

               
                await _unitOfWork.Repository<TAppSitedomain>().UpdateAsync(existingDomain);
                 
                await _unitOfWork.CompleteAsync();

                 
                return _mapper.Map<SiteDomainDto>(existingDomain); 
            }
             catch (DbUpdateException ex)
             { 
                throw new InvalidOperationException($"Alan adı veritabanında güncellenirken hata oluştu: {ex.InnerException?.Message ?? ex.Message}", ex);
             }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException || ex is ArgumentException || ex is InvalidOperationException) throw;
                throw new InvalidOperationException($"Alan adı güncellenirken hata: {ex.Message}", ex);
            }
        }
 
        /// Belirtilen ID'ye sahip alan adını pasif hale getirir. 
        public async Task DeleteDomainAsync(int id)
        { 
             if (id <= 0) {
                 throw new ArgumentException("Geçerli bir alan adı ID'si gereklidir.", nameof(id));
            }

            try
            { 
                await _unitOfWork.Repository<TAppSitedomain>().SoftDeleteAsync(id); 
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex) 
            {
                throw new InvalidOperationException($"Alan adı silinirken hata (ID: {id}): {ex.Message}", ex);
            }
        }
 
        /// Bir alan adının veritabanında aktif kayıtlar arasında benzersiz olup olmadığını kontrol eder. 
        public async Task<bool> IsDomainUniqueAsync(string domain, int? excludeDomainId = null)
        { 
            if (string.IsNullOrWhiteSpace(domain)) {
                throw new ArgumentException("Kontrol edilecek domain adı boş olamaz.", nameof(domain));
            }

            try { 
                var query = _unitOfWork.Repository<TAppSitedomain>().Query()
                    .Where(d => d.Domain == domain && d.Isdeleted == 0);  
 
                if (excludeDomainId.HasValue && excludeDomainId.Value > 0)
                {
                    query = query.Where(d => d.Id != excludeDomainId.Value);
                }
 
                return !await query.AnyAsync();
            } catch (Exception ex) { 
                throw new InvalidOperationException($"Domain benzersizliği kontrol edilirken hata ('{domain}'): {ex.Message}", ex);
            }
        }
 
        /// Belirtilen alan adına (domain) sahip aktif kaydı getirir. 
        public async Task<SiteDomainDto?> GetByDomainAsync(string domain)
        { 
             if (string.IsNullOrWhiteSpace(domain)) {
                throw new ArgumentException("Aranacak domain adı boş olamaz.", nameof(domain));
            }

            try {
                // UnitOfWork üzerinden repository sorgusu
                var domainEntity = await _unitOfWork.Repository<TAppSitedomain>().Query()
                    .FirstOrDefaultAsync(d => d.Domain == domain && d.Isdeleted == 0);  
 
                if (domainEntity == null)
                    return null;
                     
                return _mapper.Map<SiteDomainDto>(domainEntity);
            } catch (Exception ex) { 
                 throw new InvalidOperationException($"Domain '{domain}' getirilirken hata: {ex.Message}", ex);
            }
        }
    }
} 