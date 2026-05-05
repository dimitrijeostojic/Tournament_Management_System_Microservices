using FluentValidation;

namespace Application.Team.Create;

public sealed class CreateTeamRequestValidator : AbstractValidator<CreateTeamRequest>
{
    public CreateTeamRequestValidator()
    {
        RuleFor(x => x.TeamName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.GroupPublicId)
            .NotEqual(Guid.Empty);
    }
}
