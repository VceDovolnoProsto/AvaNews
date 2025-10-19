using System.Text.Json;
using AvaNews.Application.Contracts.Providers;

namespace AvaNews.Infrastructure.Providers.Polygon;

public sealed class PolygonNewsClient : INewsProviderClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _http;

    public PolygonNewsClient(HttpClient http)
    {
        _http = http;
    }

    public ProviderFeatureFlags Features => new(
        true,
        true,
        true
    );

    public async Task<ProviderPage> FetchAsync(NewsFetchRequest request, CancellationToken ct)
    {
        var requestUri = TryAsAbsoluteUri(request.PageToken, out var abs)
            ? abs
            : BuildUri("v2/reference/news", request);

        using var req = new HttpRequestMessage(HttpMethod.Get, requestUri);
        using var res = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
        res.EnsureSuccessStatusCode();

        await using var stream = await res.Content.ReadAsStreamAsync(ct);
        var payload = await JsonSerializer.DeserializeAsync<PolygonResponse>(stream, JsonOptions, ct)
                      ?? new PolygonResponse();

        var items = (payload.Results ?? [])
            .Select(r => new ProviderArticle(
                r.Id ?? Guid.NewGuid().ToString("N"),
                r.Title,
                r.Description,
                r.ArticleUrl,
                r.PublishedUtc ?? DateTimeOffset.UtcNow,
                r.Author,
                r.Publisher?.Name,
                r.Publisher?.LogoUrl,
                r.Publisher?.HomepageUrl,
                r.Tickers ?? [],
                r.Keywords,
                r.ImageUrl,
                JsonSerializer.Serialize(r, JsonOptions)
            ))
            .ToArray();

        return new ProviderPage(items, payload.NextUrl);
    }

    private static bool TryAsAbsoluteUri(string s, out Uri uri)
    {
        return Uri.TryCreate(s, UriKind.Absolute, out uri);
    }

    private Uri BuildUri(string path, NewsFetchRequest q)
    {
        var qb = new List<string>();
        if (!string.IsNullOrWhiteSpace(q.Ticker)) qb.Add($"ticker={Uri.EscapeDataString(q.Ticker)}");

        if (q.PublishedUtcFrom.HasValue)
            qb.Add($"published_utc.gte={Uri.EscapeDataString(q.PublishedUtcFrom.Value.ToString("O"))}");
        if (q.PublishedUtcTo.HasValue)
            qb.Add($"published_utc.lte={Uri.EscapeDataString(q.PublishedUtcTo.Value.ToString("O"))}");

        qb.Add("sort=published_utc");
        qb.Add("order=asc");

        if (q.Limit > 0) qb.Add($"limit={q.Limit}");

        if (!string.IsNullOrWhiteSpace(q.PageToken) && !TryAsAbsoluteUri(q.PageToken, out _))
            qb.Add($"cursor={Uri.EscapeDataString(q.PageToken!)}");

        var queryString = qb.Count > 0 ? "?" + string.Join("&", qb) : string.Empty;
        return new Uri(path + queryString, UriKind.Relative);
    }
}