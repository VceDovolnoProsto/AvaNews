namespace AvaNews.Application.Models;

public sealed record CreateSubscriptionRequest(
    IReadOnlyList<string> Tickers,
    string QueryText,
    string Channel,
    string Target
);