namespace AvaNews.Application.Contracts.Providers;

public sealed record ProviderFeatureFlags(
    bool SupportsTickerFilter,
    bool SupportsPublishedRange,
    bool SupportsCursorPaging
);