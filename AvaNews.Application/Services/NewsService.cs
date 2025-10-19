using AvaNews.Application.Contracts;
using AvaNews.Application.Models;
using AvaNews.Domain.Entities;

namespace AvaNews.Application.Services;

public sealed class NewsService : INewsService
{
    private readonly INewsRepository _repo;

    public NewsService(INewsRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<NewsItemDto>> GetAllNewsAsync(CancellationToken ct)
    {
        var items = await _repo.GetAllNewsAsync(ct);

        return items.Select(Map).ToList();
    }

    public async Task<List<NewsItemDto>> GetAllNewsWithGivingDayAsync(int days, CancellationToken ct)
    {
        var items = await _repo.GetAllNewsWithGivingDayAsync(days, ct);

        return items.Select(Map).ToList();
    }

    public async Task<List<NewsItemDto>> GetAllNewsPerInstrumentAsync(string ticker, int limit, CancellationToken ct)
    {
        var items = await _repo.GetAllNewsPerInstrumentAsync(ticker, limit, ct);

        return items.Select(Map).ToList();
    }

    public Task<List<NewsItemDto>> SearchAsync(string query, CancellationToken ct)
    {
        //The most efficient way is to create raw SQL (jsonb + windows)
        //use FromSqlRaw
        //or
        //ILike
        //For performance, add GIN indexes

        throw new NotImplementedException(); //to-do
    }

    public Task<List<NewsItemDto>> GetLatestNewsAsync(int limit, CancellationToken ct)
    {
        //The most efficient way is to create raw SQL (jsonb + windows)
        //to get one of the most recent materials per ticker, then take the top N by time
        //use FromSqlRaw

        throw new NotImplementedException(); //to-do
    }

    private static NewsItemDto Map(NewsItem e)
    {
        return new NewsItemDto(
            e.Id,
            e.Title,
            e.Summary,
            e.Url,
            e.PublishedUtc,
            new PublisherInfoDto(e.Publisher.Name, e.Publisher.LogoUrl, e.Publisher.HomepageUrl),
            e.Tickers,
            e.ImageUrl,
            e.Enrichment is null
                ? new NewsEnrichmentDto(null, null, null, string.Empty)
                : new NewsEnrichmentDto(
                    e.Enrichment.PriceNow,
                    e.Enrichment.PrevClose,
                    e.Enrichment.PriceChangePct,
                    e.Enrichment.Sentiment
                )
        );
    }
}