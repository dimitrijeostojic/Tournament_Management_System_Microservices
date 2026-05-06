namespace Domain.Entities;

public sealed class TeamStanding : Entity
{
    public Guid TeamPublicId { get; private set; }
    public int Played { get; private set; }
    public int Wins { get; private set; }
    public int Losses { get; private set; }
    public int PointsFor { get; private set; }
    public int PointsAgainst { get; private set; }
    public int StandingPoints { get; private set; }

    public static TeamStanding Create(Guid teamPublicId) => new()
    {
        TeamPublicId = teamPublicId
    };

    public void Apply(
        int played, int wins, int losses,
        int pointsFor, int pointsAgainst, int standingPoints)
    {
        Played += played;
        Wins += wins;
        Losses += losses;
        PointsFor += pointsFor;
        PointsAgainst += pointsAgainst;
        StandingPoints += standingPoints;
    }
}
