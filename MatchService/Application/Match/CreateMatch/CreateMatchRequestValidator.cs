using FluentValidation;

namespace Application.Match.CreateMatch;

public sealed class CreateMatchRequestValidator : AbstractValidator<CreateMatchRequest>
{
    public CreateMatchRequestValidator()
    {
        RuleFor(x => x.HomeTeamPublicId)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.AwayTeamPublicId)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.StadiumPublicId)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.StartTime)
            .GreaterThan(DateTime.UtcNow).WithMessage("StartTime must be in the future.");
    }
}
