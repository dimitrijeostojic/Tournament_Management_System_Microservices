using Domain.Enums;

namespace Domain.Entities;

public sealed class KnockoutBracket : Entity
{
    private readonly ICollection<KnockoutMatch> _matches = [];
    public IReadOnlyCollection<KnockoutMatch> Matches => _matches.ToList().AsReadOnly();

    // seededTeams mora imati tačno 8 elemenata (seed 1..8), svaki sa PublicId i imenom
    public static KnockoutBracket Create(List<(Guid PublicId, string Name)> seededTeams)
    {
        if (seededTeams.Count != 8)
            throw new InvalidOperationException("Knockout bracket requires exactly 8 seeded teams.");

        var bracket = new KnockoutBracket();

        // Četvrtfinala: 1v8, 2v7, 3v6, 4v5
        var qfPairings = new (int home, int away)[] { (0, 7), (1, 6), (2, 5), (3, 4) };
        for (var i = 0; i < qfPairings.Length; i++)
        {
            var match = KnockoutMatch.Create(KnockoutRound.Quarterfinal, i + 1);
            var home = seededTeams[qfPairings[i].home];
            var away = seededTeams[qfPairings[i].away];
            match.AssignTeams(home.PublicId, home.Name, away.PublicId, away.Name);
            bracket._matches.Add(match);
        }

        // Polufinala (timovi se popunjavaju naknadno kroz AdvanceWinner)
        bracket._matches.Add(KnockoutMatch.Create(KnockoutRound.Semifinal, 1));
        bracket._matches.Add(KnockoutMatch.Create(KnockoutRound.Semifinal, 2));

        // Utakmica za 3. mesto
        bracket._matches.Add(KnockoutMatch.Create(KnockoutRound.ThirdPlace, 1));

        // Finale
        bracket._matches.Add(KnockoutMatch.Create(KnockoutRound.Final, 1));

        return bracket;
    }

    // Poziva se nakon što handler snimi rezultat — pronalazi sledeći meč i dodeljuje pobednika (i gubitnika za 3. mesto)
    public void AdvanceWinner(Guid completedMatchPublicId)
    {
        var completed = _matches.FirstOrDefault(m => m.PublicId == completedMatchPublicId)
            ?? throw new InvalidOperationException("Match not found in this bracket.");

        if (completed.Status != KnockoutMatchStatus.Completed)
            throw new InvalidOperationException("Match is not yet completed.");

        var winnerId = completed.WinnerPublicId!.Value;
        var winnerName = completed.HomeTeamPublicId == winnerId
            ? completed.HomeTeamName!
            : completed.AwayTeamName!;

        var loserId = completed.HomeTeamPublicId == winnerId
            ? completed.AwayTeamPublicId!.Value
            : completed.HomeTeamPublicId!.Value;
        var loserName = completed.HomeTeamPublicId == winnerId
            ? completed.AwayTeamName!
            : completed.HomeTeamName!;

        switch (completed.Round)
        {
            case KnockoutRound.Quarterfinal:
                AdvanceFromQuarterfinal(completed.MatchOrder, winnerId, winnerName);
                break;

            case KnockoutRound.Semifinal:
                AdvanceFromSemifinal(completed.MatchOrder, winnerId, winnerName, loserId, loserName);
                break;

                // Final i ThirdPlace nemaju napredovanje
        }
    }

    private void AdvanceFromQuarterfinal(int matchOrder, Guid winnerId, string winnerName)
    {
        // QF 1 i 2 → SF 1 (1=Home, 2=Away)
        // QF 3 i 4 → SF 2 (3=Home, 4=Away)
        var sfMatchOrder = matchOrder <= 2 ? 1 : 2;
        var sf = GetMatch(KnockoutRound.Semifinal, sfMatchOrder);

        if (matchOrder % 2 == 1)  // neparni → Home
            sf.AssignHomeTeam(winnerId, winnerName);
        else                       // parni → Away
            sf.AssignAwayTeam(winnerId, winnerName);
    }

    private void AdvanceFromSemifinal(int matchOrder, Guid winnerId, string winnerName, Guid loserId, string loserName)
    {
        var final = GetMatch(KnockoutRound.Final, 1);
        var thirdPlace = GetMatch(KnockoutRound.ThirdPlace, 1);

        if (matchOrder == 1)
        {
            final.AssignHomeTeam(winnerId, winnerName);
            thirdPlace.AssignHomeTeam(loserId, loserName);
        }
        else
        {
            final.AssignAwayTeam(winnerId, winnerName);
            thirdPlace.AssignAwayTeam(loserId, loserName);
        }
    }

    private KnockoutMatch GetMatch(KnockoutRound round, int matchOrder) => _matches.First(m => m.Round == round && m.MatchOrder == matchOrder);
}
