using Blog.Api.Models;
using Blog.Entities;

namespace Blog.Api.Configuration;

public static class MapperConfiguration
{
    public static void ConfigureApiMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(config =>
        {
            config.CreateMap<Post, PostDto>();
            config.CreateMap<CreatePostDto, Post>();
            config.CreateMap<UpdatePostDto, Post>();
        });
    }
}