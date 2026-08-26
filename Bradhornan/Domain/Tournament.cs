namespace Bradhornan.Domain;

public class Tournament : GameEvent
{
    public string TournamentGame { get; }
    public override string EventType => $"Turnering: {TournamentGame}";

    public Tournament(
        string title,
        DateTime startsAt,
        string location,
        int capacity,
        Member organizer,
        string tournamentGame)
        : base(title, startsAt, location, capacity, organizer)
    {
        if (string.IsNullOrWhiteSpace(tournamentGame))
            throw new ArgumentException("Ett turneringsspel måste anges.");

        TournamentGame = tournamentGame.Trim();
    }
}
