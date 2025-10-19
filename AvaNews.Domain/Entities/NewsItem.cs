namespace AvaNews.Domain.Entities;

public sealed class NewsItem : BaseEntity
{
    public string ProviderId { get; set; }
    public string Title { get; set; }
    public string Summary { get; set; }
    public string Url { get; set; }
    public DateTimeOffset PublishedUtc { get; set; }
    public PublisherInfo Publisher { get; set; }
    public string[] Tickers { get; set; } = [];
    public string[] Keywords { get; set; } = [];
    public string ImageUrl { get; set; }
    public NewsEnrichment Enrichment { get; set; }
}