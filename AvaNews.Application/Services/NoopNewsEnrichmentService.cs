using AvaNews.Application.Contracts;
using AvaNews.Domain.Entities;

namespace AvaNews.Application.Services;

public sealed class NoopNewsEnrichmentService : INewsEnrichmentService
{
    public Task<NewsItem> EnrichAsync(NewsItem item, CancellationToken ct)
    {
        return Task.FromResult(item);
    }
}