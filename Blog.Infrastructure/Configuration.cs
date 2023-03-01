using Blog.Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Infrastructure;

public static class Configuration
{
    public static void ConfigureInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<BlogContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IBlogRepository, BlogRepository>();
    }

    public static void UseInfrastructure(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<BlogContext>();
        context.Database.EnsureCreated();
    }
}