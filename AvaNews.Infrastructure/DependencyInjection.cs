using System.Net;
using System.Net.Http.Headers;
using AvaNews.Application.Contracts;
using AvaNews.Application.Contracts.Providers;
using AvaNews.Infrastructure.Persistence;
using AvaNews.Infrastructure.Providers.Polygon;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace AvaNews.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Persistence
        services.AddDbContext<NewsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("pg")));
        services.AddScoped<INewsRepository, NewsRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

        services.AddPolygonNewsProvider(configuration);

        return services;
    }

    public static IServiceCollection AddPolygonNewsProvider(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<PolygonOptions>(configuration.GetSection("Polygon"));

        services.AddHttpClient<INewsProviderClient, PolygonNewsClient>((sp, client) =>
            {
                var opt = sp.GetRequiredService<IOptions<PolygonOptions>>().Value;
                if (!string.IsNullOrWhiteSpace(opt.BaseUrl))
                    client.BaseAddress = new Uri(opt.BaseUrl);
                if (!string.IsNullOrWhiteSpace(opt.ApiKey))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", opt.ApiKey);
                client.Timeout = TimeSpan.FromSeconds(10);
            })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetTimeoutPolicy());

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(r => r.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(200 * attempt));
    }

    private static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(12);
    }
}