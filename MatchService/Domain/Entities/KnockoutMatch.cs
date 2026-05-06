using Domain.Enums;

namespace Domain.Entities;

public sealed class KnockoutMatch : Entity
{
    public int BracketId { get; private set; }
    public Guid? HomeTeamPublicId { get; private set; }
    public Guid? AwayTeamPublicId { get; private set; }
    public string? HomeTeamName { get; private set; }
    public string? AwayTeamName { get; private set; }
    public int? HomePoints { get; private set; }
    public int? AwayPoints { get; private set; }
    public KnockoutRound Round { get; private set; }
    public int MatchOrder { get; private set; }
    public KnockoutMatchStatus Status { get; private set; }
    public Guid? WinnerPublicId { get; private set; }

    // Navigation property — KnockoutBracket je u istom servisu (FK je BracketId → KnockoutBracket.Id)
    public KnockoutBracket? Bracket { get; private set; }


    public static KnockoutMatch Create(KnockoutRound round, int matchOrder)
    {
        return new KnockoutMatch
        {
            Round = round,
            MatchOrder = matchOrder,
            Status = KnockoutMatchStatus.Pending
        };
    }

    public void AssignTeams(Guid homeTeamPublicId, string homeTeamName, Guid awayTeamPublicId, string awayTeamName)
    {
        if (Status == KnockoutMatchStatus.Completed)
            throw new InvalidOperationException("Cannot assign teams to a completed match.");

        HomeTeamPublicId = homeTeamPublicId;
        HomeTeamName = homeTeamName;
        AwayTeamPublicId = awayTeamPublicId;
        AwayTeamName = awayTeamName;
        Status = KnockoutMatchStatus.Scheduled;
    }

    public void AssignHomeTeam(Guid homeTeamPublicId, string homeTeamName)
    {
        if (Status == KnockoutMatchStatus.Completed)
            throw new InvalidOperationException("Cannot assign teams to a completed match.");

        HomeTeamPublicId = homeTeamPublicId;
        HomeTeamName = homeTeamName;
        if (AwayTeamPublicId.HasValue)
            Status = KnockoutMatchStatus.Scheduled;
    }

    public void AssignAwayTeam(Guid awayTeamPublicId, string awayTeamName)
    {
        if (Status == KnockoutMatchStatus.Completed)
            throw new InvalidOperationException("Cannot assign teams to a completed match.");

        AwayTeamPublicId = awayTeamPublicId;
        AwayTeamName = awayTeamName;
        if (HomeTeamPublicId.HasValue)
            Status = KnockoutMatchStatus.Scheduled;
    }

    public void RecordResult(int homePoints, int awayPoints)
    {
        if (Status != KnockoutMatchStatus.Scheduled)
            throw new InvalidOperationException("Match must be scheduled before recording a result.");

        if (!HomeTeamPublicId.HasValue || !AwayTeamPublicId.HasValue)
            throw new InvalidOperationException("Both teams must be assigned before recording a result.");

        if (homePoints == awayPoints)
            throw new InvalidOperationException("A knockout match cannot end in a draw.");

        if (homePoints < 0 || awayPoints < 0)
            throw new InvalidOperationException("Points cannot be negative.");

        HomePoints = homePoints;
        AwayPoints = awayPoints;
        WinnerPublicId = homePoints > awayPoints ? HomeTeamPublicId : AwayTeamPublicId;
        Status = KnockoutMatchStatus.Completed;
    }
}
