namespace AvaNews.Domain.Entities;

public sealed class NewsEnrichment
{
    public decimal PriceNow { get; set; }
    public decimal PrevClose { get; set; }
    public double PriceChangePct { get; set; }
    public string Sentiment { get; set; }
}