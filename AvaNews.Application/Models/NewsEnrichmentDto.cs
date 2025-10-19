namespace AvaNews.Application.Models;

public sealed record NewsEnrichmentDto(
    decimal? PriceNow,
    decimal? PrevClose,
    double? PriceChangePct,
    string Sentiment
);