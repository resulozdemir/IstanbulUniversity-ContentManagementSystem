using Microsoft.EntityFrameworkCore;
using new_cms.Application.Interfaces;
using new_cms.Application.Services;
using new_cms.Domain.Interfaces;
using new_cms.Domain.Entities;
using new_cms.Infrastructure.Persistence;
using new_cms.Infrastructure.Persistence.Repositories;
using new_cms.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS politikası ekleniyor
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", 
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// AutoMapper yapılandırması
Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.AddAutoMapper(
    builder.Services, 
    typeof(MappingProfile).Assembly);

// Database context
builder.Services.AddDbContext<UCmsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ISiteService, SiteService>();
builder.Services.AddScoped<ISiteDomainService, SiteDomainService>();
builder.Services.AddScoped<IThemeService, ThemeService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<IComponentService, ComponentService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<INoticeService, NoticeService>();
builder.Services.AddScoped<IPageService, PageService>();
builder.Services.AddScoped<IContentPageService, ContentPageService>();
builder.Services.AddScoped<ISitemapService, SitemapService>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

// CORS middleware'i etkinleştiriliyor
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
