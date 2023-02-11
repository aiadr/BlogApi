using Microsoft.OpenApi.Models;

namespace Blog.Api.Configuration;

public static class SwaggerConfiguration
{
    private const string AuthHeaderName = "Authorization";

    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(AuthConfiguration.SchemaName, new OpenApiSecurityScheme()
            {
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Name = AuthHeaderName,
                Scheme = AuthConfiguration.SchemaName
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = AuthConfiguration.SchemaName
                        },
                        Scheme = AuthConfiguration.SchemaName,
                        Name = AuthHeaderName,
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });

        return services;
    }
}