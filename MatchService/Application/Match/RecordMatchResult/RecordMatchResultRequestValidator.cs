using FluentValidation;

namespace Application.Match.RecordMatchResult;

public sealed class RecordMatchResultRequestValidator : AbstractValidator<RecordMatchResultRequest>
{
    public RecordMatchResultRequestValidator()
    {
        RuleFor(x => x.MatchPublicId)
            .NotNull()
            .NotEqual(Guid.Empty);

        RuleFor(x => x.HomePoints)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.AwayPoints)
            .GreaterThanOrEqualTo(0);
    }
}
