using System.Text.Json.Serialization;

namespace AvaNews.Infrastructure.Providers.Polygon;

public sealed record PolygonItem
{
    [JsonPropertyName("id")] public string Id { get; init; }

    [JsonPropertyName("title")] public string Title { get; init; }

    [JsonPropertyName("description")] public string Description { get; init; }

    [JsonPropertyName("article_url")] public string ArticleUrl { get; init; }

    [JsonPropertyName("published_utc")] public DateTimeOffset? PublishedUtc { get; init; }

    [JsonPropertyName("author")] public string Author { get; init; }

    [JsonPropertyName("tickers")] public string[] Tickers { get; init; }

    [JsonPropertyName("keywords")] public string[] Keywords { get; init; }

    [JsonPropertyName("image_url")] public string ImageUrl { get; init; }

    [JsonPropertyName("publisher")] public PolygonPublisher Publisher { get; init; }
}