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
            
            CreateMap<TAppThemecomponent, ThemeComponentDto>()
                .ForMember(dest => dest.ComponentName, opt => opt.Ignore()) 
                .ReverseMap();
            
            // Component Mappings
            CreateMap<TAppComponent, ComponentDto>().ReverseMap();
            
            CreateMap<TAppSitecomponentdata, SiteComponentDataDto>()
                .ForMember(dest => dest.ComponentName, opt => opt.Ignore()) 
                .ReverseMap();
            
            // News Mappings
            CreateMap<TAppNews, NewsDto>()
                .ForMember(dest => dest.Header, opt => opt.MapFrom(src => src.Header))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.Link))
                .ForMember(dest => dest.OnDate, opt => opt.MapFrom(src => src.Ondate))
                .ForMember(dest => dest.Img, opt => opt.MapFrom(src => src.Img))
                .ForMember(dest => dest.Tag, opt => opt.MapFrom(src => src.Tag))
                .ForMember(dest => dest.InSlider, opt => opt.MapFrom(src => src.Inslider))
                .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.Siteid))
                .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.Ispublic))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
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
                .ForMember(dest => dest.OnDate, opt => opt.MapFrom(src => src.Ondate))
                .ForMember(dest => dest.Img, opt => opt.MapFrom(src => src.Img))
                .ForMember(dest => dest.Tag, opt => opt.MapFrom(src => src.Tag))
                .ForMember(dest => dest.Gallery, opt => opt.MapFrom(src => src.Gallery))
                .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.Siteid))
                .ForMember(dest => dest.Map, opt => opt.MapFrom(src => src.Map))
                .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.Ispublic))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
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
                .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.Menuorder))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Status == 1))
                .ForMember(dest => dest.Icon, opt => opt.MapFrom(src => src.Icon))
                .ForMember(dest => dest.Target, opt => opt.MapFrom(src => src.Target))
                .ForMember(dest => dest.Children, opt => opt.Ignore());
        }
    }
} 