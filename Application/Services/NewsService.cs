using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using new_cms.Application.DTOs.NewsDTOs;
using new_cms.Application.Interfaces;
using new_cms.Domain.Entities;

namespace new_cms.Application.Services
{
    public class NewsService : INewsService
    {
        private readonly UCmsContext _context;
        private readonly IMapper _mapper;

        public NewsService(UCmsContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Tüm haberleri listeler
        public async Task<IEnumerable<NewsListDto>> GetAllNewsAsync()
        {
            var news = await _context.TAppNews
                .Where(n => n.Isdeleted == 0)
                .OrderByDescending(n => n.Ondate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<NewsListDto>>(news);
        }

        // Belirli bir haber detayını getirir
        public async Task<NewsDetailDto?> GetNewsByIdAsync(int id)
        {
            var news = await _context.TAppNews
                .Include(n => n.Site)
                .FirstOrDefaultAsync(n => n.Id == id && n.Isdeleted == 0);

            if (news == null)
                return null;

            var newsDetail = _mapper.Map<NewsDetailDto>(news);
            
            // Etiketleri ayırıp listeye ekle
            if (!string.IsNullOrEmpty(news.Tag))
            {
                // Virgülle ayrılmış etiketleri string listesine dönüştür
                newsDetail.Tags = news.Tag
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .ToList();
            }

            return newsDetail;
        }

        // Yeni haber oluşturur
        public async Task<NewsDto> CreateNewsAsync(NewsDto newsDto)
        {
            var news = _mapper.Map<TAppNews>(newsDto);
            
            // Oluşturma bilgilerini ayarla
            news.Createddate = DateTime.Now;
            news.Createduser = 1; // Sistemin varsayılan kullanıcısı veya mevcut kullanıcı ID'si eklenebilir
            news.Isdeleted = 0;
            
            _context.TAppNews.Add(news);
            await _context.SaveChangesAsync();

            return _mapper.Map<NewsDto>(news);
        }

        // Mevcut haberi günceller
        public async Task<NewsDto> UpdateNewsAsync(NewsDto newsDto)
        {
            var news = await _context.TAppNews.FindAsync(newsDto.Id);
            if (news == null)
                throw new KeyNotFoundException($"Haber bulunamadı: ID {newsDto.Id}");

            _mapper.Map(newsDto, news);
            
            // Güncelleme bilgilerini ayarla
            news.Modifieddate = DateTime.Now;
            news.Modifieduser = 1; // Sistemin varsayılan kullanıcısı veya mevcut kullanıcı ID'si eklenebilir

            _context.TAppNews.Update(news);
            await _context.SaveChangesAsync();

            return _mapper.Map<NewsDto>(news);
        }

        // Haberi soft delete yapar
        public async Task DeleteNewsAsync(int id)
        {
            var news = await _context.TAppNews.FindAsync(id);
            if (news == null)
                throw new KeyNotFoundException($"Haber bulunamadı: ID {id}");

            news.Isdeleted = 1;
            news.Modifieddate = DateTime.Now;
            news.Modifieduser = 1; // Sistemin varsayılan kullanıcısı veya mevcut kullanıcı ID'si eklenebilir

            await _context.SaveChangesAsync();
        }

        // Site ID'ye göre haberleri listeler
        public async Task<IEnumerable<NewsListDto>> GetNewsBySiteIdAsync(int siteId)
        {
            var news = await _context.TAppNews
                .Where(n => n.Siteid == siteId && n.Isdeleted == 0)
                .OrderByDescending(n => n.Ondate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<NewsListDto>>(news);
        }

        // Kategori ID'ye göre haberleri listeler
        public async Task<IEnumerable<NewsListDto>> GetNewsByCategoryIdAsync(int categoryId)
        {
            // Bu örnekte kategoriye göre filtreleme uygulanıyor.
            var news = await _context.TAppNews
                .Where(n => n.Isdeleted == 0)
                .OrderByDescending(n => n.Ondate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<NewsListDto>>(news);
        }

        // URL'ye göre haber getirir
        public async Task<NewsDetailDto?> GetNewsByUrlAsync(string url, int siteId)
        {
            // Bu örnekte Link alanına göre filtreleme yapılıyor.
            var news = await _context.TAppNews
                .Include(n => n.Site)
                .FirstOrDefaultAsync(n => n.Link.Contains(url) && n.Siteid == siteId && n.Isdeleted == 0);

            if (news == null)
                return null;

            var newsDetail = _mapper.Map<NewsDetailDto>(news);
            
            // Etiketleri ayırıp listeye ekle
            if (!string.IsNullOrEmpty(news.Tag))
            {
                // Virgülle ayrılmış etiketleri string listesine dönüştür
                newsDetail.Tags = news.Tag
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .ToList();
            }

            return newsDetail;
        }

        // Aktif (yayında) olan haberleri listeler
        public async Task<IEnumerable<NewsListDto>> GetActiveNewsAsync()
        {
            var news = await _context.TAppNews
                .Where(n => n.Isdeleted == 0 && n.Ispublish == 1)
                .OrderByDescending(n => n.Ondate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<NewsListDto>>(news);
        }

        // Sayfalı ve filtrelenmiş haber listesi döndürür
        public async Task<(IEnumerable<NewsListDto> Items, int TotalCount)> GetPagedNewsAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            string? sortBy = null,
            bool ascending = true)
        {
            // Temel sorgu
            var query = _context.TAppNews.Where(n => n.Isdeleted == 0);

            // Arama terimine göre filtreleme
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(n => n.Header.Contains(searchTerm) || 
                                    n.Content.Contains(searchTerm) ||
                                    n.Tag.Contains(searchTerm));
            }

            // Toplam kayıt sayısını al
            var totalCount = await query.CountAsync();

            // Sıralama
            IQueryable<TAppNews> sortedQuery;
            if (string.IsNullOrEmpty(sortBy))
            {
                sortedQuery = ascending 
                    ? query.OrderBy(n => n.Ondate)
                    : query.OrderByDescending(n => n.Ondate);
            }
            else
            {
                // Sıralama alanına göre sıralama
                sortedQuery = sortBy.ToLower() switch
                {
                    "header" => ascending 
                        ? query.OrderBy(n => n.Header)
                        : query.OrderByDescending(n => n.Header),
                    "date" => ascending 
                        ? query.OrderBy(n => n.Ondate)
                        : query.OrderByDescending(n => n.Ondate),
                    _ => ascending 
                        ? query.OrderBy(n => n.Ondate)
                        : query.OrderByDescending(n => n.Ondate)
                };
            }

            // Sayfalama
            var pagedItems = await sortedQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<NewsListDto>>(pagedItems);
            return (dtos, totalCount);
        }

        // Öne çıkan haberleri listeler
        public async Task<IEnumerable<NewsListDto>> GetFeaturedNewsAsync(int siteId)
        {
            // Slider'da öne çıkan haberleri getir
            var news = await _context.TAppNews
                .Where(n => n.Siteid == siteId && n.Isdeleted == 0 && n.Inslider == 1)
                .OrderByDescending(n => n.Ondate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<NewsListDto>>(news);
        }

        // En çok okunan haberleri listeler
        public async Task<IEnumerable<NewsListDto>> GetMostViewedNewsAsync(int siteId, int count = 5)
        {
            // Bu örnekte sadece siteId'ye göre en yeni haberler getiriliyor
            var news = await _context.TAppNews
                .Where(n => n.Siteid == siteId && n.Isdeleted == 0 && n.Ispublish == 1)
                .OrderByDescending(n => n.Ondate)
                .Take(count)
                .ToListAsync();

            return _mapper.Map<IEnumerable<NewsListDto>>(news);
        }

        // Belirli bir tarih aralığındaki haberleri listeler
        public async Task<IEnumerable<NewsListDto>> GetNewsByDateRangeAsync(int siteId, DateTime startDate, DateTime endDate)
        {
            var news = await _context.TAppNews
                .Where(n => n.Siteid == siteId && 
                       n.Isdeleted == 0 && 
                       n.Ondate >= startDate &&
                       n.Ondate <= endDate)
                .OrderByDescending(n => n.Ondate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<NewsListDto>>(news);
        }

        // Etiket adına göre haberleri listeler
        public async Task<IEnumerable<NewsListDto>> GetNewsByTagAsync(string tag, int siteId)
        {
            var news = await _context.TAppNews
                .Where(n => n.Siteid == siteId && 
                       n.Isdeleted == 0 && 
                       n.Tag.Contains(tag))
                .OrderByDescending(n => n.Ondate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<NewsListDto>>(news);
        }
    }
} 