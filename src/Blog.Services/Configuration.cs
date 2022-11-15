using AutoMapper;
using Blog.Entities;
using Blog.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Services;

public static class Configuration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IBlogService, BlogService>();
        services.AddAutoMapper(ConfigureMapper);
    }

    internal static void ConfigureMapper(IMapperConfigurationExpression config)
    {
        config.CreateMap<Post, Post>()
            .ForMember(x => x.CreationDate, x => x.Ignore())
            .ForMember(x => x.UpdateDate, x => x.Ignore());
    }
}