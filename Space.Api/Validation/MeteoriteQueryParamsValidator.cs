using FluentValidation;
using Space.Domain.DTO;

namespace Space.Api.Validation;

public class MeteoriteQueryParamsValidator : AbstractValidator<MeteoriteQueryParams>
{
    public MeteoriteQueryParamsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 200)
            .WithMessage("PageSize must be between 1 and 200");

        RuleFor(x => x.YearFrom)
            .GreaterThan(0)
            .When(x => x.YearFrom.HasValue);

        RuleFor(x => x.YearTo)
            .GreaterThan(0)
            .When(x => x.YearTo.HasValue);

        RuleFor(x => x)
            .Must(x => x.YearTo >= x.YearFrom)
            .WithMessage("YearTo must be greater than or equal to YearFrom")
            .When(x => x.YearFrom.HasValue && x.YearTo.HasValue);

        RuleFor(x => x.SortOrder)
            .Must(x => x!.ToLower() is "asc" or "desc")
            .WithMessage("SortOrder must be 'Asc' or 'Desc'");

        RuleFor(x => x.SortField)
            .Must(x => AllowedSortFields.Contains(x!, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Invalid SortField value");
    }

    private static readonly string[] AllowedSortFields =
    {
        "year", "count", "totalmass"
    };
}
