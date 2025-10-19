using AvaNews.Application.Models;
using FluentValidation;

namespace AvaNews.Application.Validators;

public sealed class CreateSubscriptionRequestValidator : AbstractValidator<CreateSubscriptionRequest>
{
    public CreateSubscriptionRequestValidator()
    {
        RuleFor(x => x.Channel)
            .NotEmpty();

        RuleFor(x => x)
            .Must(HaveTickerOrQuery)
            .WithMessage("Specify at least tickers or queryText");

        RuleForEach(x => x.Tickers)
            .NotEmpty();

        RuleFor(x => x.Target)
            .MaximumLength(200).When(x => x.Target != null);
    }

    private static bool HaveTickerOrQuery(CreateSubscriptionRequest request)
    {
        return request.Tickers is { Count: > 0 } || !string.IsNullOrWhiteSpace(request.QueryText);
    }
}