using AvaNews.Application.Contracts;
using AvaNews.Domain.Entities;

namespace AvaNews.Infrastructure.Persistence;

public sealed class SubscriptionRepository : ISubscriptionRepository
{
    private readonly NewsDbContext _db;

    public SubscriptionRepository(NewsDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> CreateAsync(Subscription sub, CancellationToken ct)
    {
        await _db.Subscriptions.AddAsync(sub, ct);
        await _db.SaveChangesAsync(ct);
        return sub.Id;
    }
}