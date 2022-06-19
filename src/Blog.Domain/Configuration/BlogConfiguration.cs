using Blog.Domain.Contracts;
using Blog.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Domain.Configuration;

public static class BlogConfiguration
{
    public static IServiceCollection ConfigureBlog(this IServiceCollection services)
    {
        services.AddScoped<IBlogService, BlogService>();
        services.AddAutoMapper(typeof(BlogConfiguration));
        return services;
    }
}