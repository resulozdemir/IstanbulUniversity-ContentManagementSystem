using Microsoft.EntityFrameworkCore;
using new_cms.Application.Interfaces;
using new_cms.Application.Services;
using new_cms.Domain.Interfaces;
using new_cms.Domain.Entities;
using new_cms.Infrastructure.Persistence.Repositories;
using new_cms.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AutoMapper yapılandırması
Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.AddAutoMapper(
    builder.Services, 
    typeof(MappingProfile).Assembly);

// Database context
builder.Services.AddDbContext<UCmsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<ISiteRepository, SiteRepository>();
builder.Services.AddScoped<ISiteDomainRepository, SiteDomainRepository>();
builder.Services.AddScoped<IThemeRepository, ThemeRepository>();
builder.Services.AddScoped<IComponentRepository, ComponentRepository>();
builder.Services.AddScoped<ISitePageRepository, SitePageRepository>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<IContentGroupRepository, ContentGroupRepository>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();

builder.Services.AddScoped<ISiteService, SiteService>();
builder.Services.AddScoped<ISiteDomainService, SiteDomainService>();
builder.Services.AddScoped<IThemeService, ThemeService>();
builder.Services.AddScoped<IComponentService, ComponentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
