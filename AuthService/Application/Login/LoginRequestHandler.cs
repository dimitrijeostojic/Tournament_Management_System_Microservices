using Application.Common;
using Core;
using Domain.Abstraction;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Login;

public sealed class LoginRequestHandler(
    UserManager<User> userManager,
    IJwtTokenRepository jwtTokenRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork
    )
    : IRequestHandler<LoginRequest, Result<LoginResponse>>
{
    private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly IJwtTokenRepository _jwtTokenRepository = jwtTokenRepository ?? throw new ArgumentNullException(nameof(jwtTokenRepository));
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<Result<LoginResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Result<LoginResponse>.Failure(ApplicationErrors.InvalidCredentials);
        }

        if (user.TwoFactorEnabled)
        {
            return Result<LoginResponse>.Success(
                new LoginResponse(null, null, RequiresTwoFactor: true, UserId: user.Id));
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains(Core.Roles.Admin) && !roles.Contains(Core.Roles.Manager))
        {
            return Result<LoginResponse>.Failure(ApplicationErrors.UnauthorizedRole);
        }

        var accessToken = await _jwtTokenRepository.GenerateTokenAsync(user, roles);
        var refreshToken = Domain.Entities.RefreshToken.Create(user.Id);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<LoginResponse>.Success(new LoginResponse(accessToken, refreshToken.Token));
    }
}
