using AvaNews.Application.Contracts;
using AvaNews.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AvaNews.Infrastructure.Persistence;

public sealed class NewsRepository : INewsRepository
{
    private readonly NewsDbContext _context;

    public NewsRepository(NewsDbContext context)
    {
        _context = context;
    }

    public async Task UpsertRangeAsync(IEnumerable<NewsItem> items, CancellationToken ct)
    {
        var list = items.ToList();
        if (list.Count == 0) return;

        var providerIds = list.Select(i => i.ProviderId).ToArray();
        var existing = await _context.News
            .Where(n => providerIds.Contains(n.ProviderId))
            .ToDictionaryAsync(n => n.ProviderId, ct);

        foreach (var item in list)
            if (!existing.TryGetValue(item.ProviderId, out var ex))
                _context.News.Add(item);
            else
                _context.Entry(ex).CurrentValues.SetValues(new
                {
                    item.Title,
                    item.Summary,
                    item.Url,
                    item.PublishedUtc,
                    item.Publisher,
                    item.Tickers,
                    item.Keywords,
                    item.ImageUrl,
                    item.Enrichment
                });
        await _context.SaveChangesAsync(ct);
    }

    public Task<List<string>> GetExistingProviderIdsAsync(IEnumerable<string> providerIds, CancellationToken ct)
    {
        return _context.News.AsNoTracking()
            .Where(n => providerIds.Contains(n.ProviderId))
            .Select(n => n.ProviderId)
            .ToListAsync(ct);
    }

    public async Task<DateTimeOffset?> GetLatestPublishedUtcAsync(CancellationToken ct)
    {
        return await _context.News.AsNoTracking()
            .MaxAsync(n => (DateTimeOffset?)n.PublishedUtc, ct);
    }

    public async Task<List<NewsItem>> GetAllNewsAsync(CancellationToken ct)
    {
        var news = await _context.News
            .Include(n => n.Publisher)
            .Include(n => n.Enrichment)
            .AsNoTracking()
            .ToListAsync(ct);

        return news;
    }

    public async Task<List<NewsItem>> GetAllNewsWithGivingDayAsync(int days, CancellationToken ct)
    {
        var news = await _context.News
            .Where(n => n.PublishedUtc >= DateTimeOffset.UtcNow.AddDays(-days))
            .Include(n => n.Publisher)
            .Include(n => n.Enrichment)
            .AsNoTracking()
            .ToListAsync(ct);

        return news;
    }

    public async Task<List<NewsItem>> GetAllNewsPerInstrumentAsync(string ticker, int limit, CancellationToken ct)
    {
        var news = await _context.News
            .Include(n => n.Publisher)
            .Include(n => n.Enrichment)
            .Where(n => n.Tickers.Contains(ticker))
            .Take(limit)
            .AsNoTracking()
            .ToListAsync(ct);

        return news;
    }
}