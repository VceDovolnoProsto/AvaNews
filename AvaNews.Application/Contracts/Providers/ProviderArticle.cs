namespace AvaNews.Application.Contracts.Providers;

public sealed record ProviderArticle(
    string ProviderId,
    string Title,
    string Description,
    string ArticleUrl,
    DateTimeOffset PublishedUtc,
    string Author,
    string PublisherName,
    string PublisherLogoUrl,
    string PublisherHomepageUrl,
    IReadOnlyList<string> Tickers,
    IReadOnlyList<string> Keywords,
    string ImageUrl,
    string RawJson
);