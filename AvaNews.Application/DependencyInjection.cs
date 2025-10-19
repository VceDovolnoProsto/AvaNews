using AvaNews.Application.Contracts;
using AvaNews.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AvaNews.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<INewsService, NewsService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<INewsIngestionService, NewsIngestionService>();
        services.AddScoped<INewsEnrichmentService, NoopNewsEnrichmentService>();
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}