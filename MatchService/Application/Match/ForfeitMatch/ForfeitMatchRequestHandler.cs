using Application.Common;
using Application.Events;
using Core;
using Domain.Abstraction;
using Domain.RepositoryInterfaces;
using MassTransit;
using MediatR;

namespace Application.Match.ForfeitMatch;

public sealed class ForfeitMatchRequestHandler(
    IMatchRepository matchRepository,
    IUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint)
    : IRequestHandler<ForfeitMatchRequest, Result<ForfeitMatchResponse>>
{
    public async Task<Result<ForfeitMatchResponse>> Handle(
        ForfeitMatchRequest request,
        CancellationToken cancellationToken)
    {
        var match = await matchRepository.GetByPublicIdAsync(request.MatchPublicId!.Value, cancellationToken);

        if (match is null)
            return Result<ForfeitMatchResponse>.Failure(ApplicationErrors.NotFound);

        match.Forfeit(request.ForfeitLoser!.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var delta = match.CalculateStandingsDelta();

        await publishEndpoint.Publish(new MatchCompletedEvent
        {
            MatchPublicId = match.PublicId,
            HomeTeamPublicId = delta.HomeTeamPublicId,
            AwayTeamPublicId = delta.AwayTeamPublicId,
            HomePlayed = delta.HomePlayed,
            HomeWins = delta.HomeWins,
            HomeLosses = delta.HomeLosses,
            HomePointsFor = delta.HomePointsFor,
            HomePointsAgainst = delta.HomePointsAgainst,
            HomeStandingPoints = delta.HomeStandingPoints,
            AwayPlayed = delta.AwayPlayed,
            AwayWins = delta.AwayWins,
            AwayLosses = delta.AwayLosses,
            AwayPointsFor = delta.AwayPointsFor,
            AwayPointsAgainst = delta.AwayPointsAgainst,
            AwayStandingPoints = delta.AwayStandingPoints
        }, cancellationToken);

        return Result<ForfeitMatchResponse>.Success(new ForfeitMatchResponse
        {
            PublicId = match.PublicId
        });
    }
}
