namespace AvaNews.Domain.Entities;

public sealed class Subscription : BaseEntity
{
    public string UserId { get; set; }
    public IReadOnlyList<string> Tickers { get; set; }
    public string QueryText { get; set; }
    public string Channel { get; set; }
    public string Target { get; set; }
    public bool IsActive { get; set; } = true;
}