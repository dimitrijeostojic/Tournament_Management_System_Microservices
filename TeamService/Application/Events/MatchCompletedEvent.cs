namespace Application.Events;

public sealed record MatchCompletedEvent
{
    public Guid MatchPublicId { get; init; }
    public Guid HomeTeamPublicId { get; init; }
    public Guid AwayTeamPublicId { get; init; }
    public int HomePlayed { get; init; }
    public int HomeWins { get; init; }
    public int HomeLosses { get; init; }
    public int HomePointsFor { get; init; }
    public int HomePointsAgainst { get; init; }
    public int HomeStandingPoints { get; init; }
    public int AwayPlayed { get; init; }
    public int AwayWins { get; init; }
    public int AwayLosses { get; init; }
    public int AwayPointsFor { get; init; }
    public int AwayPointsAgainst { get; init; }
    public int AwayStandingPoints { get; init; }
}