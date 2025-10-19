using System.Text.Json.Serialization;

namespace AvaNews.Infrastructure.Providers.Polygon;

public sealed record PolygonResponse
{
    [JsonPropertyName("results")] public List<PolygonItem> Results { get; init; }

    [JsonPropertyName("next_url")] public string NextUrl { get; init; }
}