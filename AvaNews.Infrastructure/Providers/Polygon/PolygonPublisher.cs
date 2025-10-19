using System.Text.Json.Serialization;

namespace AvaNews.Infrastructure.Providers.Polygon;

public sealed record PolygonPublisher
{
    [JsonPropertyName("name")] public string Name { get; init; }

    [JsonPropertyName("logo_url")] public string LogoUrl { get; init; }

    [JsonPropertyName("homepage_url")] public string HomepageUrl { get; init; }
}