using AvaNews.Application.Contracts;
using AvaNews.Application.Contracts.Providers;
using AvaNews.Application.Options;
using AvaNews.Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace AvaNews.Application.Services;

public sealed class NewsIngestionService : INewsIngestionService
{
    private readonly IValidator<ProviderArticle> _articleValidator;
    private readonly INewsEnrichmentService _newsEnrichment;
    private readonly IngestionOptions _opt;
    private readonly INewsProviderClient _provider;
    private readonly INewsRepository _repo;

    public NewsIngestionService(
        INewsProviderClient provider,
        INewsRepository repo,
        INewsEnrichmentService newsEnrichment,
        IOptions<IngestionOptions> opt,
        IValidator<ProviderArticle> articleValidator)
    {
        _provider = provider;
        _repo = repo;
        _newsEnrichment = newsEnrichment;
        _opt = opt.Value;
        _articleValidator = articleValidator;
    }

    public async Task<int> IngestOnceAsync(CancellationToken ct)
    {
        var latest = await _repo.GetLatestPublishedUtcAsync(ct);
        var now = DateTimeOffset.UtcNow;

        var fromByLatest = latest?.AddMinutes(-_opt.LookbackOverlapMinutes);
        var fromByInitial = now.AddMinutes(-_opt.InitialLookbackMinutes);

        var effectiveFrom = fromByLatest ?? fromByInitial;

        var ingested = 0;
        var pages = 0;
        string cursor = null;

        do
        {
            ct.ThrowIfCancellationRequested();

            var request = new NewsFetchRequest(
                null,
                _provider.Features.SupportsPublishedRange ? effectiveFrom : null,
                null,
                cursor,
                _opt.PageLimit
            );

            var page = await _provider.FetchAsync(request, ct);

            if (page.Items.Count == 0)
                break;

            var ids = page.Items.Select(i => i.ProviderId).ToArray();
            var existing = await _repo.GetExistingProviderIdsAsync(ids, ct);
            var fresh = page.Items.Where(i => !existing.Contains(i.ProviderId)).ToList();

            if (fresh.Count == 0)
            {
                cursor = page.NextPageToken;
                pages++;
                continue;
            }

            var mapped = new List<NewsItem>(fresh.Count);
            foreach (var a in fresh)
            {
                await _articleValidator.ValidateAndThrowAsync(a, ct);

                var item = MapToDomain(a);
                try
                {
                    item = await _newsEnrichment.EnrichAsync(item, ct);
                }
                catch (Exception)
                {
                    // newsEnrichment
                }

                mapped.Add(item);
            }

            await _repo.UpsertRangeAsync(mapped, ct);
            ingested += mapped.Count;

            cursor = page.NextPageToken;
            pages++;
        } while (!string.IsNullOrWhiteSpace(cursor) && pages < _opt.MaxPagesPerRun);

        return ingested;
    }

    private static NewsItem MapToDomain(ProviderArticle providerArticle)
    {
        var tickers = (providerArticle.Tickers ?? Array.Empty<string>())
            .Select(t => t?.Trim().ToUpperInvariant())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct()
            .ToArray();

        var keywords = (providerArticle.Keywords ?? Array.Empty<string>())
            .Select(k => k?.Trim())
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Distinct()
            .ToArray();

        return new NewsItem
        {
            ProviderId = providerArticle.ProviderId,
            Title = providerArticle.Title,
            Url = providerArticle.ArticleUrl,
            PublishedUtc = providerArticle.PublishedUtc,
            Publisher = new PublisherInfo
            {
                Name = providerArticle.PublisherName,
                LogoUrl = providerArticle.PublisherLogoUrl,
                HomepageUrl = providerArticle.PublisherHomepageUrl
            },
            Tickers = tickers,
            Summary = providerArticle.Description,
            Keywords = keywords,
            ImageUrl = providerArticle.ImageUrl
        };
    }
}