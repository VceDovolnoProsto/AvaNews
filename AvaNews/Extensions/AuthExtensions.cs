using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace AvaNews.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration cfg)
    {
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        var authority = cfg["Jwt:Authority"];
        var audience = cfg["Jwt:Audience"];

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = audience;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
                // For local dev IdPs without HTTPS metadata endpoints, uncomment:
                // options.RequireHttpsMetadata = false;
                options.SaveToken = true;
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("NewsWrite", policy =>
            {
                policy.RequireAssertion(ctx =>
                {
                    var scopes = ctx.User.Claims
                        .Where(c => c.Type is "scope" or "scp")
                        .SelectMany(c => c.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);
                    return scopes.Contains("news.write");
                });
            });
        });

        // Swagger: Bearer support
        services.AddSwaggerGen(c =>
        {
            var scheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Input: Bearer {token}"
            };
            c.AddSecurityDefinition("Bearer", scheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { scheme, Array.Empty<string>() }
            });
        });

        return services;
    }
}