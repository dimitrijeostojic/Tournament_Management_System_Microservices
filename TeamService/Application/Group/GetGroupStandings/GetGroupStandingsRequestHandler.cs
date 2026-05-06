using Application.Common;
using Core;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.Group.GetGroupStandings;

public sealed class GetGroupStandingsRequestHandler(
    ITeamRepository teamRepository,
    ITeamStandingRepository standingRepository)
    : IRequestHandler<GetGroupStandingsRequest, Result<GetGroupStandingsResponse>>
{
    public async Task<Result<GetGroupStandingsResponse>> Handle(
        GetGroupStandingsRequest request,
        CancellationToken cancellationToken)
    {
        var teams = await teamRepository.GetByGroupPublicIdAsync(
            request.GroupPublicId, cancellationToken);

        if (teams.Count == 0)
            return Result<GetGroupStandingsResponse>.Failure(ApplicationErrors.NotFound);

        var standings = await standingRepository.GetByGroupPublicIdAsync(
            request.GroupPublicId, cancellationToken);

        var standingMap = standings.ToDictionary(s => s.TeamPublicId);

        var dtos = teams.Select(t =>
        {
            standingMap.TryGetValue(t.PublicId, out var standing);
            return new StandingDto
            {
                TeamPublicId = t.PublicId,
                TeamName = t.TeamName,
                Played = standing?.Played ?? 0,
                Wins = standing?.Wins ?? 0,
                Losses = standing?.Losses ?? 0,
                PointsFor = standing?.PointsFor ?? 0,
                PointsAgainst = standing?.PointsAgainst ?? 0,
                StandingPoints = standing?.StandingPoints ?? 0
            };
        })
        .OrderByDescending(x => x.StandingPoints)
        .ThenByDescending(x => x.PointsFor - x.PointsAgainst)
        .ThenByDescending(x => x.PointsFor)
        .ToList();

        return Result<GetGroupStandingsResponse>.Success(
            new GetGroupStandingsResponse(dtos));
    }
}