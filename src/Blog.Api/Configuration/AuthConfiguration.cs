using Blog.Api.Auth;

namespace Blog.Api.Configuration;

public static class AuthConfiguration
{
    public const string SchemaName = "Basic";

    public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(SchemaName)
            .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(
                SchemaName,
                options =>
                {
                    var configSection = configuration.GetSection("Auth");
                    configSection.Bind(options);
                });

        return services;
    }
}