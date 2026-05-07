using Application.Common;
using Core;
using Domain.Abstraction;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.TwoFactor.Verify;

public sealed class VerifyTwoFactorRequestHandler(
    UserManager<User> userManager,
    IJwtTokenRepository jwtTokenRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork
    )
    : IRequestHandler<VerifyTwoFactorRequest, Result<VerifyTwoFactorResponse>>
{
    private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly IJwtTokenRepository _jwtTokenRepository = jwtTokenRepository ?? throw new ArgumentNullException(nameof(jwtTokenRepository));
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<Result<VerifyTwoFactorResponse>> Handle(VerifyTwoFactorRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null)
            return Result<VerifyTwoFactorResponse>.Failure(ApplicationErrors.NotFound);

        if (!user.TwoFactorEnabled)
            return Result<VerifyTwoFactorResponse>.Failure(ApplicationErrors.TwoFactorNotEnabled);

        var isValid = await _userManager.VerifyTwoFactorTokenAsync(
            user,
            _userManager.Options.Tokens.AuthenticatorTokenProvider,
            request.Code);

        if (!isValid)
            return Result<VerifyTwoFactorResponse>.Failure(ApplicationErrors.InvalidTwoFactorCode);

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains(Roles.Admin) && !roles.Contains(Roles.Manager))
            return Result<VerifyTwoFactorResponse>.Failure(ApplicationErrors.UnauthorizedRole);
        var accessToken = await _jwtTokenRepository.GenerateTokenAsync(user, roles);
        var refreshToken = Domain.Entities.RefreshToken.Create(user.Id);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<VerifyTwoFactorResponse>.Success(
            new VerifyTwoFactorResponse(accessToken, refreshToken.Token!));
    }
}
