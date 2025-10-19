using AvaNews.Domain.Entities;

namespace AvaNews.Application.Contracts;

public interface INewsRepository
{
    Task UpsertRangeAsync(IEnumerable<NewsItem> items, CancellationToken ct);

    Task<List<string>> GetExistingProviderIdsAsync(IEnumerable<string> providerIds, CancellationToken ct);

    Task<DateTimeOffset?> GetLatestPublishedUtcAsync(CancellationToken ct);

    Task<List<NewsItem>> GetAllNewsAsync(CancellationToken ct);

    Task<List<NewsItem>> GetAllNewsWithGivingDayAsync(int days, CancellationToken ct);

    Task<List<NewsItem>> GetAllNewsPerInstrumentAsync(string ticker, int limit, CancellationToken ct);
}