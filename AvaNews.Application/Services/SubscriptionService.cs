using AvaNews.Application.Contracts;
using AvaNews.Application.Models;
using AvaNews.Domain.Entities;
using FluentValidation;

namespace AvaNews.Application.Services;

public sealed class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _repo;
    private readonly IValidator<CreateSubscriptionRequest> _validator;

    public SubscriptionService(ISubscriptionRepository repo, IValidator<CreateSubscriptionRequest> validator)
    {
        _repo = repo;
        _validator = validator;
    }

    public async Task<CreateSubscriptionResponse> CreateAsync(string userId, CreateSubscriptionRequest request,
        CancellationToken ct)
    {
        await _validator.ValidateAndThrowAsync(request, ct);

        var sub = new Subscription
        {
            UserId = userId,
            Channel = request.Channel,
            Target = request.Target,
            QueryText = request.QueryText,
            Tickers = request.Tickers,
            IsActive = true
        };

        var id = await _repo.CreateAsync(sub, ct);

        return new CreateSubscriptionResponse(id);
    }
}