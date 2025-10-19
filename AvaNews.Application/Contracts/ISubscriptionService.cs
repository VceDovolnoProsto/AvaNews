using AvaNews.Application.Models;

namespace AvaNews.Application.Contracts;

public interface ISubscriptionService
{
    Task<CreateSubscriptionResponse> CreateAsync(string userId, CreateSubscriptionRequest request, CancellationToken ct);
}