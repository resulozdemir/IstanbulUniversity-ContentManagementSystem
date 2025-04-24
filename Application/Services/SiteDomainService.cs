using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using new_cms.Application.DTOs.SiteDTOs;
using new_cms.Application.Interfaces;
using new_cms.Domain.Entities;
using new_cms.Domain.Interfaces;

namespace new_cms.Application.Services
{
    /// Site alan adı yönetimi için servis sınıfı.
    /// Site alan adlarını (domain) listeleme, ekleme, düzenleme ve silme işlemlerini gerçekleştirir.
    public class SiteDomainService : ISiteDomainService
    {
        private readonly ISiteDomainRepository _siteDomainRepository;
        private readonly IMapper _mapper;

        public SiteDomainService(ISiteDomainRepository siteDomainRepository, IMapper mapper)
        {
            
            _siteDomainRepository = siteDomainRepository ?? throw new ArgumentNullException(nameof(siteDomainRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// Belirli bir siteye ait tüm alan adlarını listeler
        public async Task<IEnumerable<SiteDomainDto>> GetDomainsBySiteIdAsync(int siteId)
        {
            // Site ID'ye göre alan adlarını getir
            var domains = await _siteDomainRepository.GetDomainsBySiteIdAsync(siteId);
            
            // Entity'leri DTO'ya dönüştür
            return _mapper.Map<IEnumerable<SiteDomainDto>>(domains);
        }

        /// ID'ye göre alan adı detayını getirir
        public async Task<SiteDomainDto?> GetDomainByIdAsync(int id)
        {
            // ID'ye göre alan adı bilgisini getir
            var domain = await _siteDomainRepository.GetDomainByIdAsync(id);
            if (domain == null)
                return null;
                
            // Entity'i DTO'ya dönüştür
            return _mapper.Map<SiteDomainDto>(domain);
        }

        /// Yeni alan adı ekler
        public async Task<SiteDomainDto> CreateDomainAsync(SiteDomainDto domainDto)
        {
            try
            {
                // DTO'yu entity'e dönüştür
                var domain = _mapper.Map<TAppSitedomain>(domainDto);
                
                // Oluşturma tarihi ve kullanıcı bilgilerini set et
                domain.Createddate = DateTime.UtcNow;
                domain.Createduser = 1; // Aktif kullanıcı ID'si servis üzerinden alınabilir
                domain.Isdeleted = 0;
                
                // Veritabanına ekle
                await _siteDomainRepository.AddAsync(domain);
                
                // Eklenen entity'i DTO'ya dönüştürerek geri dön
                return _mapper.Map<SiteDomainDto>(domain);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Alan adı eklenirken hata: {ex.Message}", ex);
            }
        }

        /// Mevcut alan adını günceller
        public async Task<SiteDomainDto> UpdateDomainAsync(SiteDomainDto domainDto)
        {
            try
            {
                // ID kontrolü
                if (domainDto.Id == null)
                    throw new ArgumentNullException(nameof(domainDto.Id), "Alan adı ID'si gereklidir");
                
                // Mevcut kaydı getir
                var existingDomain = await _siteDomainRepository.GetByIdAsync(domainDto.Id.Value);
                if (existingDomain == null)
                    throw new InvalidOperationException($"ID: {domainDto.Id} olan alan adı bulunamadı");
                
                // Mevcut kaydı güncelle
                _mapper.Map(domainDto, existingDomain);
                
                // Güncelleme tarihi ve kullanıcı bilgilerini set et
                existingDomain.Modifieddate = DateTime.UtcNow;
                existingDomain.Modifieduser = 1; // Aktif kullanıcı ID'si servis üzerinden alınabilir
                
                // Veritabanında güncelle
                await _siteDomainRepository.UpdateAsync(existingDomain);
                
                // Güncellenen entity'i DTO'ya dönüştürerek geri dön
                return _mapper.Map<SiteDomainDto>(existingDomain);
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
                // Mevcut kaydı getir
                var domain = await _siteDomainRepository.GetByIdAsync(id);
                if (domain == null)
                    throw new InvalidOperationException($"ID: {id} olan alan adı bulunamadı");
                
                // Soft delete işlemi
                domain.Isdeleted = 1;
                domain.Modifieddate = DateTime.UtcNow;
                domain.Modifieduser = 1; // Aktif kullanıcı ID'si servis üzerinden alınabilir
                
                // Veritabanında güncelle
                await _siteDomainRepository.UpdateAsync(domain);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Alan adı silinirken hata: {ex.Message}", ex);
            }
        }

        /// Alan adının benzersiz olup olmadığını kontrol eder
        public async Task<bool> IsDomainUniqueAsync(string domain, int? excludeDomainId = null)
        {
            return await _siteDomainRepository.IsDomainUniqueAsync(domain, excludeDomainId);
        }

        /// Alan adına göre kayıt getirir
        public async Task<SiteDomainDto?> GetByDomainAsync(string domain)
        {
            // Alan adına göre kayıt getir
            var domainEntity = await _siteDomainRepository.GetByDomainAsync(domain);
            if (domainEntity == null)
                return null;
                
            // Entity'i DTO'ya dönüştür
            return _mapper.Map<SiteDomainDto>(domainEntity);
        }

        /// Belirli bir dile ait tüm alan adlarını listeler
        public async Task<IEnumerable<SiteDomainDto>> GetDomainsByLanguageAsync(string language)
        {
            // Dile göre alan adlarını getir
            var domains = await _siteDomainRepository.GetDomainsByLanguageAsync(language);
            
            // Entity'leri DTO'ya dönüştür
            return _mapper.Map<IEnumerable<SiteDomainDto>>(domains);
        }
    }
} 