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
    /// Site alan adlarını (domain) listeleme, ekleme, düzenleme ve silme işlemlerini gerçekleştirir.
    public class SiteDomainService : ISiteDomainService
    {
        private readonly IRepository<TAppSitedomain> _siteDomainRepository;
        private readonly IMapper _mapper;

        public SiteDomainService(IRepository<TAppSitedomain> siteDomainRepository, IMapper mapper)
        {
            _siteDomainRepository = siteDomainRepository;
            _mapper = mapper;
        }

        /// Belirli bir siteye ait tüm alan adlarını listeler
        public async Task<IEnumerable<SiteDomainDto>> GetDomainsBySiteIdAsync(int siteId)
        {
            var domains = await _siteDomainRepository.Query()
                .Where(d => d.Siteid == siteId && d.Isdeleted == 0)
                .ToListAsync();

            return _mapper.Map<IEnumerable<SiteDomainDto>>(domains);
        }

        /// Belirli bir dile ait tüm alan adlarını listeler
        public async Task<IEnumerable<SiteDomainDto>> GetDomainsByLanguageAsync(string language)
        {
            var domains = await _siteDomainRepository.Query()
                .Where(d => d.Isdeleted == 0)
                .ToListAsync();

            return _mapper.Map<IEnumerable<SiteDomainDto>>(domains);
        }

        /// Yeni alan adı ekler
        public async Task<SiteDomainDto> CreateDomainAsync(SiteDomainDto domainDto)
        {
            if (string.IsNullOrWhiteSpace(domainDto.Domain))
                throw new ArgumentException("Domain alanı boş olamaz.", nameof(domainDto.Domain));
            if (domainDto.SiteId <= 0) // SiteId entity'de int olduğu için DTO'da da int varsayıyoruz
                 throw new ArgumentException("Geçerli bir Site ID belirtilmelidir.", nameof(domainDto.SiteId));
            if (string.IsNullOrWhiteSpace(domainDto.Language))
                throw new ArgumentException("Dil alanı boş olamaz.", nameof(domainDto.Language));
             if (string.IsNullOrWhiteSpace(domainDto.Key))
                 throw new ArgumentException("Key alanı boş olamaz.", nameof(domainDto.Key));


            if (!await IsDomainUniqueAsync(domainDto.Domain))
                throw new InvalidOperationException($"'{domainDto.Domain}' alan adı zaten kullanılıyor.");

            try
            {
                var domain = _mapper.Map<TAppSitedomain>(domainDto);

                domain.Createddate = DateTime.UtcNow;
                domain.Isdeleted = 0;
                // TODO: domain.Createduser = GetCurrentUserId();

                var createdDomain = await _siteDomainRepository.AddAsync(domain);
                return _mapper.Map<SiteDomainDto>(createdDomain); 
            }
            catch (DbUpdateException ex) 
            {
                throw new InvalidOperationException($"Alan adı veritabanına eklenirken hata oluştu: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Alan adı eklenirken beklenmedik bir hata oluştu: {ex.Message}", ex);
            }
        }

        /// Mevcut alan adını günceller
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
                var existingDomain = await _siteDomainRepository.GetByIdAsync(domainDto.Id.Value);
                if (existingDomain == null || existingDomain.Isdeleted == 1)
                    throw new KeyNotFoundException($"ID: {domainDto.Id} olan alan adı bulunamadı veya silinmiş.");

                 // Domain adı değiştirilemez, bu yüzden benzersizlik kontrolü kaldırıldı.

                // Kritik alanları koru
                var originalDomain = existingDomain.Domain; // Domain'i de koru
                var originalIsDeleted = existingDomain.Isdeleted;
                var originalCreatedDate = existingDomain.Createddate;
                var originalCreatedUser = existingDomain.Createduser;

                _mapper.Map(domainDto, existingDomain);

                // Korunan değerleri geri yükle
                existingDomain.Domain = originalDomain; // Orijinal domain'i geri yükle
                existingDomain.Isdeleted = originalIsDeleted;
                existingDomain.Createddate = originalCreatedDate;
                existingDomain.Createduser = originalCreatedUser;

                existingDomain.Modifieddate = DateTime.UtcNow;
                //existingDomain.Modifieduser = GetCurrentUserId();

                await _siteDomainRepository.UpdateAsync(existingDomain);
                return _mapper.Map<SiteDomainDto>(existingDomain); 
            }
             catch (DbUpdateException ex)
             {
                 throw new InvalidOperationException($"Alan adı veritabanında güncellenirken hata oluştu: {ex.InnerException?.Message ?? ex.Message}", ex);
             }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Alan adı güncellenirken hata: {ex.Message}", ex);
            }
        }

        /// Alan adını soft delete yapar
        public async Task DeleteDomainAsync(int id)
        {
            try
            {
                await _siteDomainRepository.SoftDeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Alan adı silinirken hata: {ex.Message}", ex);
            }
        }

        /// Alan adının benzersiz olup olmadığını kontrol eder
        public async Task<bool> IsDomainUniqueAsync(string domain, int? excludeDomainId = null)
        {
            var query = _siteDomainRepository.Query()
                .Where(d => d.Domain == domain && d.Isdeleted == 0);

            if (excludeDomainId.HasValue)
            {
                query = query.Where(d => d.Id != excludeDomainId.Value);
            }

            return !await query.AnyAsync();
        }

        /// Alan adına göre kayıt getirir
        public async Task<SiteDomainDto?> GetByDomainAsync(string domain)
        {
            var domainEntity = await _siteDomainRepository.Query()
                .FirstOrDefaultAsync(d => d.Domain == domain && d.Isdeleted == 0);

            if (domainEntity == null)
                return null;
                
            return _mapper.Map<SiteDomainDto>(domainEntity);
        }
    }
} 