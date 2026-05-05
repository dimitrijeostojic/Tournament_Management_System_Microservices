using FluentValidation;

namespace Application.Team.Update;

public sealed class UpdateTeamRequestValidator : AbstractValidator<UpdateTeamRequest>
{
    public UpdateTeamRequestValidator()
    {
        RuleFor(x => x.TeamPublicId)
            .NotNull()
            .Must(id => id != Guid.Empty).WithMessage("TeamPublicId must not be an empty Guid.");

        RuleFor(x => x.TeamName)
            .NotEmpty()
            .MaximumLength(100)
            .When(x => x.TeamName != null);
    }
}
