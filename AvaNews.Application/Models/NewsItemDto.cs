namespace AvaNews.Application.Models;

public sealed record NewsItemDto(
    Guid Id,
    string Title,
    string Summary,
    string Url,
    DateTimeOffset PublishedUtc,
    PublisherInfoDto Publisher,
    IReadOnlyList<string> Tickers,
    string ImageUrl,
    NewsEnrichmentDto Enrichment
);