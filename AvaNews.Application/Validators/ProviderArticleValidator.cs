using AvaNews.Application.Contracts.Providers;
using FluentValidation;

namespace AvaNews.Application.Validators;

public sealed class ProviderArticleValidator : AbstractValidator<ProviderArticle>
{
    public ProviderArticleValidator()
    {
        RuleFor(x => x.ProviderId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty();

        RuleFor(x => x.ArticleUrl)
            .NotEmpty();

        RuleFor(x => x.PublishedUtc)
            .Must(dt => dt != default)
            .WithMessage("PublishedUtc must be set");

        RuleFor(x => x.Tickers)
            .NotNull();

        // Optional fields: Description, Keywords, ImageUrl, RawJson can be null
    }
}