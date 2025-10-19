namespace AvaNews.Application.Contracts.Providers;

public sealed record NewsFetchRequest(
    string Ticker = null,
    DateTimeOffset? PublishedUtcFrom = null,
    DateTimeOffset? PublishedUtcTo = null,
    string PageToken = null,
    int Limit = 100
);