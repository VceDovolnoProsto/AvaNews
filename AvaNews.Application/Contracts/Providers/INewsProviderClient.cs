namespace AvaNews.Application.Contracts.Providers;

public interface INewsProviderClient
{
    ProviderFeatureFlags Features { get; }

    Task<ProviderPage> FetchAsync(
        NewsFetchRequest request,
        CancellationToken ct);
}