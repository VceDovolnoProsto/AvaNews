using AvaNews.Domain.Entities;

namespace AvaNews.Application.Contracts;

public interface ISubscriptionRepository
{
    Task<Guid> CreateAsync(Subscription sub, CancellationToken ct);
}