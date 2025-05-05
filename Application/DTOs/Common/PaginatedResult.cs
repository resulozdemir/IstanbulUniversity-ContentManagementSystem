using System;
using System.Collections.Generic;

namespace new_cms.Application.DTOs.Common
{
    /// API'de sayfalama (pagination) işlemleri için kullanılan DTO sınıfıdır.
    /// Toplam kayıt sayısı, mevcut sayfa, sayfa boyutu ve veri listesini taşır.
    public class PaginatedResult<T>
    {
        /// Sayfadaki veri listesi
        public IEnumerable<T> Items { get; }

        /// Toplam kayıt sayısı
        public int TotalCount { get; }
        
        /// Mevcut sayfa numarası
        public int PageNumber { get; }
        
        /// Sayfa başına gösterilecek kayıt sayısı
        public int PageSize { get; }
        
        /// Toplam sayfa sayısı
        public int TotalPages { get; }

        /// Sayfalama sonucu oluşturucu
        public PaginatedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }
    }
} 