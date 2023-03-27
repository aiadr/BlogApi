using Blog.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Repository;

public static class Configuration
{
    public static void ConfigureInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<BlogContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IPostsRepository, PostsRepository>();
    }

    public static void UseInfrastructure(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<BlogContext>();
        context.Database.EnsureCreated();
    }
}