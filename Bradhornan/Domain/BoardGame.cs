namespace Bradhornan.Domain;

public class BoardGame
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Title { get; private set; } = string.Empty;
    public GameCategory Category { get; private set; }
    public int MinPlayers { get; private set; }
    public int MaxPlayers { get; private set; }
    public int PlayTimeMinutes { get; private set; }
    public DifficultyLevel Difficulty { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public GameAvailability Availability { get; private set; } = GameAvailability.Available;
    public Guid? ReservedForEventId { get; private set; }

    public string PlayerRange => $"{MinPlayers}-{MaxPlayers}";

    public BoardGame(
        string title,
        GameCategory category,
        int minPlayers,
        int maxPlayers,
        int playTimeMinutes,
        DifficultyLevel difficulty,
        string description = "")
    {
        UpdateDetails(title, category, minPlayers, maxPlayers, playTimeMinutes, difficulty, description);
    }

    public void UpdateDetails(
        string title,
        GameCategory category,
        int minPlayers,
        int maxPlayers,
        int playTimeMinutes,
        DifficultyLevel difficulty,
        string description)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Spelets titel måste anges.");

        if (minPlayers < 1 || maxPlayers < minPlayers)
            throw new ArgumentException("Antalet spelare är ogiltigt.");

        if (playTimeMinutes <= 0)
            throw new ArgumentException("Speltiden måste vara större än noll.");

        Title = title.Trim();
        Category = category;
        MinPlayers = minPlayers;
        MaxPlayers = maxPlayers;
        PlayTimeMinutes = playTimeMinutes;
        Difficulty = difficulty;
        Description = description?.Trim() ?? string.Empty;
    }

    public bool SupportsPlayerCount(int playerCount) =>
        playerCount >= MinPlayers && playerCount <= MaxPlayers;

    public void ReserveFor(GameEvent gameEvent)
    {
        if (Availability != GameAvailability.Available)
            throw new InvalidOperationException($"{Title} är inte tillgängligt för reservation.");

        Availability = GameAvailability.Reserved;
        ReservedForEventId = gameEvent.Id;
    }

    public void ReleaseReservation(Guid eventId)
    {
        if (ReservedForEventId != eventId)
            return;

        Availability = GameAvailability.Available;
        ReservedForEventId = null;
    }

    public void MarkUnavailable()
    {
        if (Availability == GameAvailability.Reserved)
            throw new InvalidOperationException("Ett reserverat spel måste först tas bort från spelträffen.");

        Availability = GameAvailability.Unavailable;
    }

    public void MarkAvailable()
    {
        Availability = GameAvailability.Available;
        ReservedForEventId = null;
    }

    public override string ToString() => $"{Title} ({Category})";
}
