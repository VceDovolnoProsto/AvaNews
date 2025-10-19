namespace AvaNews.Application.Contracts.Providers;

public sealed record ProviderPage(
    IReadOnlyList<ProviderArticle> Items,
    string NextPageToken
);