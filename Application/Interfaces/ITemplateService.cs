using new_cms.Application.DTOs.SiteDTOs;
using new_cms.Application.DTOs.TemplateDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace new_cms.Application.Interfaces
{
    public interface ITemplateService
    {
        Task<TemplateDto> CreateTemplateAsync(TemplateDto templateDto);
        Task<IEnumerable<TemplateDto>> GetAllTemplatesAsync();
        Task<TemplateDto?> GetTemplateByIdAsync(int templateId);
        Task<TemplateDto> UpdateTemplateAsync(int templateId, TemplateDto templateDto);
        Task DeleteTemplateAsync(int templateId);

        // Belirli bir şablonu kullanan tüm aktif siteleri listeler.
        Task<IEnumerable<SiteListDto>> GetSitesByTemplateAsync(int templateId);

        // Bir kaynak şablonun içeriğini (sayfalar, bileşenler vb.) hedef bir siteye kopyalar.
        // sourceTemplateId: İçeriği kopyalanacak kaynak şablonun ID'si.
        // targetSiteId: İçeriğin kopyalanacağı hedef sitenin ID'si.
        Task CopyTemplateContentToSiteAsync(int sourceTemplateId, int targetSiteId);
    }
} 