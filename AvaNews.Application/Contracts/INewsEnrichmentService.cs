using AvaNews.Domain.Entities;

namespace AvaNews.Application.Contracts;

public interface INewsEnrichmentService
{
    Task<NewsItem> EnrichAsync(NewsItem item, CancellationToken ct);
}