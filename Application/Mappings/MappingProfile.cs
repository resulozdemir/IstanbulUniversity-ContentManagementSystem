using AutoMapper;
using new_cms.Domain.Entities;
using new_cms.Application.DTOs.SiteDTOs;
using new_cms.Application.DTOs.EventDTOs;
using new_cms.Application.DTOs.MenuDTOs;
using new_cms.Application.DTOs.NewsDTOs;
using new_cms.Application.DTOs.NoticeDTOs;
using new_cms.Application.DTOs.ThemeDTOs;
using new_cms.Application.DTOs.ComponentDTOs;
using new_cms.Application.DTOs.PageDTOs;
using new_cms.Application.DTOs.TemplateDTOs;
using new_cms.Application.DTOs.ContentPageDTOs;
using new_cms.Application.DTOs.SitemapDTOs;
using new_cms.Application.DTOs.UploadDTOs;

namespace new_cms.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Site Mappings
            CreateMap<TAppSite, SiteDto>().ReverseMap();
                
            CreateMap<TAppSite, SiteListDto>()
                .ForMember(dest => dest.ThemeName, opt => opt.Ignore()) 
                .ReverseMap();
                
            CreateMap<TAppSite, SiteDetailDto>()
                .ForMember(dest => dest.ThemeName, opt => opt.Ignore())
                .ForMember(dest => dest.Domains, opt => opt.Ignore()) 
                .ReverseMap();
                
            CreateMap<TAppSitedomain, SiteDomainDto>().ReverseMap();
            
            // Theme Mappings
            CreateMap<TAppTheme, ThemeDto>().ReverseMap();
            
            CreateMap<TAppThemecomponent, ThemeComponentDto>().ReverseMap();
            
            // Template Mappings
            CreateMap<TAppSite, TemplateDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ThemeId, opt => opt.MapFrom(src => src.Themeid))
                .ForMember(dest => dest.ThemeName, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Istemplate, opt => opt.MapFrom(src => 1)) // 1: Bu bir şablondur
                .ForMember(dest => dest.Createddate, opt => opt.Ignore())
                .ForMember(dest => dest.Createduser, opt => opt.Ignore())
                .ForMember(dest => dest.Modifieddate, opt => opt.Ignore())
                .ForMember(dest => dest.Modifieduser, opt => opt.Ignore())
                .ForMember(dest => dest.Isdeleted, opt => opt.Ignore());
            
            // Component Mappings
            CreateMap<TAppComponent, ComponentDto>()
                .ForMember(dest => dest.TagName, opt => opt.MapFrom(src => src.Tagname))
                .ReverseMap()
                .ForMember(dest => dest.Tagname, opt => opt.MapFrom(src => src.TagName))
                .ForMember(dest => dest.Createddate, opt => opt.Ignore())
                .ForMember(dest => dest.Createduser, opt => opt.Ignore())
                .ForMember(dest => dest.Modifieddate, opt => opt.Ignore())
                .ForMember(dest => dest.Modifieduser, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
            
            CreateMap<TAppSitecomponentdata, SiteComponentDataDto>()
                .ForMember(dest => dest.ComponentName, opt => opt.Ignore()) 
                .ReverseMap();
            
            // Sitemap Mappings
            CreateMap<TAppSitemap, SitemapDto>()
                .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.Siteid))
                .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.Itemid))
                .ForMember(dest => dest.RedirectTo, opt => opt.MapFrom(src => src.Redirectto))
                .ForMember(dest => dest.CreatedUser, opt => opt.MapFrom(src => src.Createduser))
                .ForMember(dest => dest.ModifiedUser, opt => opt.MapFrom(src => src.Modifieduser))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.Isdeleted))
                .ReverseMap()
                .ForMember(dest => dest.Siteid, opt => opt.MapFrom(src => src.SiteId))
                .ForMember(dest => dest.Itemid, opt => opt.MapFrom(src => src.ItemId))
                .ForMember(dest => dest.Redirectto, opt => opt.MapFrom(src => src.RedirectTo))
                .ForMember(dest => dest.Createddate, opt => opt.Ignore())
                .ForMember(dest => dest.Createduser, opt => opt.Ignore())
                .ForMember(dest => dest.Modifieddate, opt => opt.Ignore())
                .ForMember(dest => dest.Modifieduser, opt => opt.Ignore())
                .ForMember(dest => dest.Isdeleted, opt => opt.Ignore());

            CreateMap<TAppSitemap, SitemapListDto>()
                .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.Siteid))
                .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.Itemid))
                .ForMember(dest => dest.RedirectTo, opt => opt.MapFrom(src => src.Redirectto))
                .ForMember(dest => dest.CreatedUser, opt => opt.MapFrom(src => src.Createduser))
                .ForMember(dest => dest.ModifiedUser, opt => opt.MapFrom(src => src.Modifieduser))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.Isdeleted));
                
            // ContentPage Mappings
            CreateMap<TAppContentpage, ContentPageDto>()
                .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.Groupid))
                .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.Siteid))
                .ForMember(dest => dest.OrderBy, opt => opt.MapFrom(src => src.Orderby))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.Createddate))
                .ForMember(dest => dest.CreatedUser, opt => opt.MapFrom(src => src.Createduser))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.Modifieddate))
                .ForMember(dest => dest.ModifiedUser, opt => opt.MapFrom(src => src.Modifieduser))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
                .ReverseMap()
                .ForMember(dest => dest.Groupid, opt => opt.MapFrom(src => src.GroupId))
                .ForMember(dest => dest.Siteid, opt => opt.MapFrom(src => src.SiteId))
                .ForMember(dest => dest.Orderby, opt => opt.MapFrom(src => src.OrderBy))
                .ForMember(dest => dest.Createddate, opt => opt.Ignore())
                .ForMember(dest => dest.Createduser, opt => opt.Ignore())
                .ForMember(dest => dest.Modifieddate, opt => opt.Ignore())
                .ForMember(dest => dest.Modifieduser, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<TAppContentpage, ContentPageListDto>()
                .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.Groupid))
                .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.Siteid))
                .ForMember(dest => dest.OrderBy, opt => opt.MapFrom(src => src.Orderby))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.Createddate))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.Modifieddate))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted));
            
                        // Page Mappings - Tüm property'ler doğru şekilde map ediliyor
            CreateMap<TAppSitepage, PageDto>()
                .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.Siteid))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.Isdeleted))
                .ForMember(dest => dest.Html, opt => opt.MapFrom(src => src.Html))
                .ForMember(dest => dest.Style, opt => opt.MapFrom(src => src.Style))
                .ForMember(dest => dest.Javascript, opt => opt.MapFrom(src => src.Javascript))
                .ForMember(dest => dest.Routing, opt => opt.MapFrom(src => src.Routing))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ParentId, opt => opt.Ignore()) // Entity'de bu alan yok
                .ForMember(dest => dest.MetaTitle, opt => opt.Ignore()) // Entity'de bu alan yok
                .ForMember(dest => dest.MetaDescription, opt => opt.Ignore()) // Entity'de bu alan yok
                .ForMember(dest => dest.MetaKeywords, opt => opt.Ignore()) // Entity'de bu alan yok
                .ForMember(dest => dest.ShowInMenu, opt => opt.Ignore()) // Entity'de bu alan yok
                .ForMember(dest => dest.MenuOrder, opt => opt.Ignore()) // Entity'de bu alan yok
                .ForMember(dest => dest.Layout, opt => opt.Ignore()); // Entity'de bu alan yok
                
            CreateMap<PageDto, TAppSitepage>()
                .ForMember(dest => dest.Siteid, opt => opt.MapFrom(src => src.SiteId))
                .ForMember(dest => dest.Isdeleted, opt => opt.MapFrom(src => src.IsDeleted))
                .ForMember(dest => dest.Html, opt => opt.MapFrom(src => src.Html))
                .ForMember(dest => dest.Style, opt => opt.MapFrom(src => src.Style))
                .ForMember(dest => dest.Javascript, opt => opt.MapFrom(src => src.Javascript))
                .ForMember(dest => dest.Routing, opt => opt.MapFrom(src => src.Routing))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Createddate, opt => opt.Ignore())
                .ForMember(dest => dest.Createduser, opt => opt.Ignore())
                .ForMember(dest => dest.Modifieddate, opt => opt.Ignore())
                .ForMember(dest => dest.Modifieduser, opt => opt.Ignore())
                .ForMember(dest => dest.Templateid, opt => opt.Ignore())
                .ForMember(dest => dest.Isdefault, opt => opt.Ignore())
                .ForMember(dest => dest.Virtualpage, opt => opt.Ignore())
                .ForMember(dest => dest.Readonly, opt => opt.Ignore())
                .ForMember(dest => dest.Column3, opt => opt.Ignore())
                .ForMember(dest => dest.Column4, opt => opt.Ignore())
                .ForMember(dest => dest.Htmldev, opt => opt.Ignore())
                .ForMember(dest => dest.Readonly, opt => opt.Ignore())
                .ForMember(dest => dest.Styledev, opt => opt.Ignore())
                .ForMember(dest => dest.Javascriptdev, opt => opt.Ignore())
                .ForMember(dest => dest.Site, opt => opt.Ignore());

            CreateMap<TAppSitepage, PageListDto>()
                .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.Siteid))
                .ForMember(dest => dest.SiteName, opt => opt.Ignore()) // Join ile doldurulacak
                .ForMember(dest => dest.ParentId, opt => opt.Ignore()) // Entity'de bu alan yok
                .ForMember(dest => dest.ParentName, opt => opt.Ignore()) // Join ile doldurulacak
                .ForMember(dest => dest.Routing, opt => opt.MapFrom(src => src.Routing))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.Createddate))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.Isdeleted))
                .ForMember(dest => dest.ShowInMenu, opt => opt.Ignore()) // Entity'de bu alan yok
                .ForMember(dest => dest.MenuOrder, opt => opt.Ignore()) // Entity'de bu alan yok
                .ForMember(dest => dest.ChildCount, opt => opt.Ignore()); // Servis katmanında hesaplanacak

            CreateMap<TAppSitepage, PageDetailDto>()
                .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.Siteid))
                .ForMember(dest => dest.SiteName, opt => opt.Ignore()) // Join ile doldurulacak
                .ForMember(dest => dest.ParentId, opt => opt.Ignore()) // Entity'de bu alan yok
                .ForMember(dest => dest.ParentName, opt => opt.Ignore()) // Join ile doldurulacak
                .ForMember(dest => dest.Routing, opt => opt.MapFrom(src => src.Routing))
                .ForMember(dest => dest.MetaTitle, opt => opt.Ignore()) // Entity'de bu alan yok
                .ForMember(dest => dest.MetaDescription, opt => opt.Ignore()) // Entity'de bu alan yok
                .ForMember(dest => dest.MetaKeywords, opt => opt.Ignore()) // Entity'de bu alan yok
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.Createddate))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.Modifieddate))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.Isdeleted))
                .ForMember(dest => dest.ShowInMenu, opt => opt.Ignore()) // Entity'de bu alan yok
                .ForMember(dest => dest.MenuOrder, opt => opt.Ignore()) // Entity'de bu alan yok
                .ForMember(dest => dest.Layout, opt => opt.Ignore()) // Entity'de bu alan yok
                .ForMember(dest => dest.ChildPages, opt => opt.Ignore()); // Servis katmanında doldurulacak


            // News Mappings
            CreateMap<TAppNews, NewsDto>()
                .ForMember(dest => dest.Header, opt => opt.MapFrom(src => src.Header))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.Link)) 
                .ForMember(dest => dest.Img, opt => opt.MapFrom(src => src.Img))
                .ForMember(dest => dest.Tag, opt => opt.MapFrom(src => src.Tag))
                .ForMember(dest => dest.InSlider, opt => opt.MapFrom(src => src.Inslider))
                .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.Siteid))
                .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.Ispublic)) 
                .ForMember(dest => dest.ContentInner, opt => opt.MapFrom(src => src.Contentinner))
                .ForMember(dest => dest.PriorityOrder, opt => opt.MapFrom(src => src.Priorityorder))
                .ForMember(dest => dest.IsPublish, opt => opt.MapFrom(src => src.Ispublish))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.Isdeleted))
                .ReverseMap()
                .ForMember(dest => dest.Createddate, opt => opt.Ignore())
                .ForMember(dest => dest.Createduser, opt => opt.Ignore())
                .ForMember(dest => dest.Modifieddate, opt => opt.Ignore())
                .ForMember(dest => dest.Modifieduser, opt => opt.Ignore());
                
            CreateMap<TAppNews, NewsListDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Header))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Img))
                .ForMember(dest => dest.PublishDate, opt => opt.MapFrom(src => src.Ondate))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Ispublish == 1))
                .ReverseMap();
            
            // Event Mappings
            CreateMap<TAppEvent, EventDto>()
                .ForMember(dest => dest.Header, opt => opt.MapFrom(src => src.Header))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
                .ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.Link)) 
                .ForMember(dest => dest.Img, opt => opt.MapFrom(src => src.Img))
                .ForMember(dest => dest.Tag, opt => opt.MapFrom(src => src.Tag))
                .ForMember(dest => dest.Gallery, opt => opt.MapFrom(src => src.Gallery))
                .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.Siteid))
                .ForMember(dest => dest.Map, opt => opt.MapFrom(src => src.Map))
                .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.Ispublic)) 
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.ContentInner, opt => opt.MapFrom(src => src.Contentinner))
                .ForMember(dest => dest.PriorityOrder, opt => opt.MapFrom(src => src.Priorityorder))
                .ForMember(dest => dest.IsPublish, opt => opt.MapFrom(src => src.Ispublish))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.Isdeleted))
                .ReverseMap()
                .ForMember(dest => dest.Createddate, opt => opt.Ignore())
                .ForMember(dest => dest.Createduser, opt => opt.Ignore())
                .ForMember(dest => dest.Modifieddate, opt => opt.Ignore())
                .ForMember(dest => dest.Modifieduser, opt => opt.Ignore());
                
            CreateMap<TAppEvent, EventListDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Header))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Img))
                .ForMember(dest => dest.EventDate, opt => opt.MapFrom(src => src.Ondate))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Ispublish == 1))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Address))
                .ReverseMap();
            
            // Notice Mappings
            CreateMap<TAppNotice, NoticeDto>().ReverseMap()
                .ForMember(dest => dest.Createddate, opt => opt.Ignore())
                .ForMember(dest => dest.Createduser, opt => opt.Ignore())
                .ForMember(dest => dest.Modifieddate, opt => opt.Ignore())
                .ForMember(dest => dest.Modifieduser, opt => opt.Ignore())
                .ForMember(dest => dest.Isdeleted, opt => opt.Ignore());

            CreateMap<TAppNotice, NoticeListDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Header))
                .ForMember(dest => dest.PublishDate, opt => opt.MapFrom(src => src.Ondate))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Img))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Ispublish == 1));
            
            // Menu Mappings
            CreateMap<TAppMenu, MenuDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.Siteid))
                .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.Parentid))
                .ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.Link))
                .ForMember(dest => dest.MenuOrder, opt => opt.MapFrom(src => src.Menuorder))
                .ForMember(dest => dest.Icon, opt => opt.MapFrom(src => src.Icon))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.Groupid))
                .ForMember(dest => dest.Target, opt => opt.MapFrom(src => src.Target))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.Isdeleted))
                .ReverseMap();
                
            CreateMap<TAppMenu, MenuListDto>()
                .ForMember(dest => dest.ParentName, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Status == 1)) 
                .ReverseMap();
                
            CreateMap<TAppMenu, MenuTreeDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Link))
                .ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => src.Menuorder))
                .ForMember(dest => dest.IsVisible, opt => opt.MapFrom(src => src.Status == 1))
                .ForMember(dest => dest.Icon, opt => opt.MapFrom(src => src.Icon))
                .ForMember(dest => dest.Target, opt => opt.MapFrom(src => src.Target))
                .ForMember(dest => dest.Children, opt => opt.Ignore());

            // Upload Mappings
            CreateMap<TAppUpload, UploadDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Userid))
                .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.Siteid))
                .ForMember(dest => dest.ImageId, opt => opt.MapFrom(src => src.Imageid))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.Createddate))
                .ForMember(dest => dest.CreatedUser, opt => opt.MapFrom(src => src.Createduser))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.Modifieddate))
                .ForMember(dest => dest.ModifiedUser, opt => opt.MapFrom(src => src.Modifieduser))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.Isdeleted))
                .ReverseMap()
                .ForMember(dest => dest.Userid, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Siteid, opt => opt.MapFrom(src => src.SiteId))
                .ForMember(dest => dest.Imageid, opt => opt.MapFrom(src => src.ImageId))
                .ForMember(dest => dest.Createddate, opt => opt.Ignore())
                .ForMember(dest => dest.Createduser, opt => opt.Ignore())
                .ForMember(dest => dest.Modifieddate, opt => opt.Ignore())
                .ForMember(dest => dest.Modifieduser, opt => opt.Ignore())
                .ForMember(dest => dest.Isdeleted, opt => opt.Ignore());

            CreateMap<TAppUploadfile, UploadFileDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Userid))
                .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.Siteid))
                .ForMember(dest => dest.FileId, opt => opt.MapFrom(src => src.Fileid))
                .ForMember(dest => dest.Salt, opt => opt.MapFrom(src => src.Salt))
                .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.Path))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.Filesize))
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.Filename))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.Createddate))
                .ForMember(dest => dest.CreatedUser, opt => opt.MapFrom(src => src.Createduser))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.Modifieddate))
                .ForMember(dest => dest.ModifiedUser, opt => opt.MapFrom(src => src.Modifieduser))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.Isdeleted))
                .ReverseMap()
                .ForMember(dest => dest.Userid, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Siteid, opt => opt.MapFrom(src => src.SiteId))
                .ForMember(dest => dest.Fileid, opt => opt.MapFrom(src => src.FileId))
                .ForMember(dest => dest.Filesize, opt => opt.MapFrom(src => src.FileSize))
                .ForMember(dest => dest.Filename, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dest => dest.Createddate, opt => opt.Ignore())
                .ForMember(dest => dest.Createduser, opt => opt.Ignore())
                .ForMember(dest => dest.Modifieddate, opt => opt.Ignore())
                .ForMember(dest => dest.Modifieduser, opt => opt.Ignore())
                .ForMember(dest => dest.Isdeleted, opt => opt.Ignore());

            CreateMap<TAppUploadapp, UploadAppDto>()
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Key))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.Path))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.Createddate))
                .ForMember(dest => dest.CreatedUser, opt => opt.MapFrom(src => src.Createduser))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.Modifieddate))
                .ForMember(dest => dest.ModifiedUser, opt => opt.MapFrom(src => src.Modifieduser))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.Isdeleted))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.MaxWidth, opt => opt.MapFrom(src => src.Maxwidth))
                .ForMember(dest => dest.MaxHeight, opt => opt.MapFrom(src => src.Maxheight))
                .ForMember(dest => dest.ThumbnailSize, opt => opt.MapFrom(src => src.Thumbnailsize))
                .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.Filepath))
                .ReverseMap()
                .ForMember(dest => dest.Maxwidth, opt => opt.MapFrom(src => src.MaxWidth))
                .ForMember(dest => dest.Maxheight, opt => opt.MapFrom(src => src.MaxHeight))
                .ForMember(dest => dest.Thumbnailsize, opt => opt.MapFrom(src => src.ThumbnailSize))
                .ForMember(dest => dest.Filepath, opt => opt.MapFrom(src => src.FilePath))
                .ForMember(dest => dest.Createddate, opt => opt.Ignore())
                .ForMember(dest => dest.Createduser, opt => opt.Ignore())
                .ForMember(dest => dest.Modifieddate, opt => opt.Ignore())
                .ForMember(dest => dest.Modifieduser, opt => opt.Ignore())
                .ForMember(dest => dest.Isdeleted, opt => opt.Ignore());
        }
    }
} 