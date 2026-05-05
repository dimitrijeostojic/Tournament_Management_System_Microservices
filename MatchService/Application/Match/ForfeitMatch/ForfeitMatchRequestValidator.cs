using FluentValidation;

namespace Application.Match.ForfeitMatch;

public sealed class ForfeitMatchRequestValidator : AbstractValidator<ForfeitMatchRequest>
{
    public ForfeitMatchRequestValidator()
    {
        RuleFor(x => x.MatchPublicId)
            .NotNull()
            .NotEqual(Guid.Empty);

        RuleFor(x => x.ForfeitLoser)
            .NotNull()
            .IsInEnum();
    }
}
