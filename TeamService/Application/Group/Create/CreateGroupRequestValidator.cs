using FluentValidation;

namespace Application.Group.Create;

public sealed class CreateGroupRequestValidator : AbstractValidator<CreateGroupRequest>
{
    public CreateGroupRequestValidator()
    {
        RuleFor(x => x.GroupName)
            .NotEmpty()
            .MaximumLength(100);
    }
}
