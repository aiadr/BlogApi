using AutoMapper;
using Blog.Repository.Contracts.Models;
using Blog.Services.Contracts;
using Blog.Services.Contracts.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Services;

public static class Configuration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IPostsService, PostsService>();
        services.AddAutoMapper(ConfigureMapper);
    }

    internal static void ConfigureMapper(IMapperConfigurationExpression config)
    {
        config.CreateMap<Post, PostDto>();
        config.CreateMap<EditPostDto, Post>();
    }
}