using Application.Common;
using Core;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Registration;

public sealed class RegisterRequestHandler(UserManager<User> userManager)
    : IRequestHandler<RegisterRequest, Result<RegisterResponse>>
{
    private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

    public async Task<Result<RegisterResponse>> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            return Result<RegisterResponse>.Failure(ApplicationErrors.EmailAlreadyExists);

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName,
            Email = request.Email
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
            return Result<RegisterResponse>.Failure(
                new Error("Register.Error", createResult.Errors.First().Description));

        var roleResult = await _userManager.AddToRoleAsync(user, Roles.Manager);
        if (!roleResult.Succeeded)
            return Result<RegisterResponse>.Failure(
                new Error("Register.Error", roleResult.Errors.First().Description));

        return Result<RegisterResponse>.Success(new RegisterResponse("Manager registered successfully."));
    }
}
