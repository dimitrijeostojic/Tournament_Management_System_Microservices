using Application.Common;
using Application.Events;
using Core;
using Domain.Abstraction;
using Domain.RepositoryInterfaces;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Match.RecordMatchResult;

public sealed class RecordMatchResultRequestHandler(
    IMatchRepository matchRepository,
    IUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint)
    : IRequestHandler<RecordMatchResultRequest, Result<RecordMatchResultResponse>>
{
    public async Task<Result<RecordMatchResultResponse>> Handle(
        RecordMatchResultRequest request,
        CancellationToken cancellationToken)
    {
        var match = await matchRepository.GetByPublicIdAsync(request.MatchPublicId!.Value, cancellationToken);

        if (match is null)
            return Result<RecordMatchResultResponse>.Failure(ApplicationErrors.NotFound);

        match.RecordResult(DateTime.UtcNow, request.HomePoints, request.AwayPoints);

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

        return Result<RecordMatchResultResponse>.Success(new RecordMatchResultResponse
        {
            PublicId = match.PublicId
        });
    }
}