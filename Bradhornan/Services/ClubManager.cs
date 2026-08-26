using Bradhornan.Domain;

namespace Bradhornan.Services;

public class ClubManager
{
    private readonly List<Member> _members = new();
    private readonly List<BoardGame> _games = new();
    private readonly List<GameEvent> _events = new();

    public IReadOnlyList<Member> Members => _members.AsReadOnly();
    public IReadOnlyList<BoardGame> Games => _games.AsReadOnly();
    public IReadOnlyList<GameEvent> Events => _events.AsReadOnly();

    public Member AddMember(string name, string email, DateTime? joinedOn = null)
    {
        if (_members.Any(m => m.Email.Equals(email?.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Det finns redan en medlem med denna e-postadress.");

        int nextMemberNumber = _members.Count == 0 ? 1001 : _members.Max(m => m.MemberNumber) + 1;
        var member = new Member(nextMemberNumber, name, email, joinedOn ?? DateTime.Today);
        _members.Add(member);
        return member;
    }

    public void RemoveMember(Member member)
    {
        bool isUsed = _events.Any(e =>
            e.Organizer.Id == member.Id ||
            e.Registrations.Any(r => r.Member.Id == member.Id));

        if (isUsed)
            throw new InvalidOperationException(
                "Medlemmen finns kopplad till en spelträff och kan därför inte tas bort. Markera medlemmen som inaktiv i stället.");

        _members.Remove(member);
    }

    public void UpdateMember(Member member, string name, string email)
    {
        if (_members.Any(m => m.Id != member.Id &&
                              m.Email.Equals(email?.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Det finns redan en annan medlem med denna e-postadress.");

        member.UpdateContactDetails(name, email);
    }

    public BoardGame AddGame(
        string title,
        GameCategory category,
        int minPlayers,
        int maxPlayers,
        int playTimeMinutes,
        DifficultyLevel difficulty,
        string description = "")
    {
        if (_games.Any(g => g.Title.Equals(title?.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Det finns redan ett spel med denna titel.");

        var game = new BoardGame(
            title, category, minPlayers, maxPlayers, playTimeMinutes, difficulty, description);

        _games.Add(game);
        return game;
    }

    public void RemoveGame(BoardGame game)
    {
        if (game.Availability == GameAvailability.Reserved)
            throw new InvalidOperationException("Ett reserverat spel kan inte tas bort.");

        _games.Remove(game);
    }

    public void UpdateGame(
        BoardGame game,
        string title,
        GameCategory category,
        int minPlayers,
        int maxPlayers,
        int playTimeMinutes,
        DifficultyLevel difficulty,
        string description)
    {
        if (_games.Any(g => g.Id != game.Id &&
                            g.Title.Equals(title?.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Det finns redan ett annat spel med denna titel.");

        game.UpdateDetails(
            title, category, minPlayers, maxPlayers, playTimeMinutes, difficulty, description);
    }

    public GameEvent AddEvent(
        string title,
        DateTime startsAt,
        string location,
        int capacity,
        Member organizer,
        string eventType,
        string extraInformation)
    {
        if (startsAt <= DateTime.Now)
            throw new ArgumentException("Spelträffen måste ha ett datum och en tid i framtiden.");

        GameEvent gameEvent = eventType switch
        {
            "Temakväll" => new ThemeNight(
                title, startsAt, location, capacity, organizer, extraInformation),
            "Turnering" => new Tournament(
                title, startsAt, location, capacity, organizer, extraInformation),
            _ => new OpenGameNight(title, startsAt, location, capacity, organizer)
        };

        _events.Add(gameEvent);
        return gameEvent;
    }

    public void RemoveEvent(GameEvent gameEvent)
    {
        foreach (BoardGame game in gameEvent.PlannedGames.ToList())
            gameEvent.RemovePlannedGame(game);

        _events.Remove(gameEvent);
    }

    // LINQ-filtrering
    public IEnumerable<Member> SearchMembers(string searchText) =>
        _members
            .Where(m => string.IsNullOrWhiteSpace(searchText) ||
                        m.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                        m.Email.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            .OrderBy(m => m.Name);

    // LINQ-filtrering och sortering
    public IEnumerable<BoardGame> SearchGames(string searchText, GameCategory? category = null) =>
        _games
            .Where(g => string.IsNullOrWhiteSpace(searchText) ||
                        g.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            .Where(g => category is null || g.Category == category)
            .OrderBy(g => g.Title);

    // LINQ-gruppering
    public IEnumerable<IGrouping<GameCategory, BoardGame>> GetGamesGroupedByCategory() =>
        _games
            .OrderBy(g => g.Title)
            .GroupBy(g => g.Category)
            .OrderBy(group => group.Key);

    public IEnumerable<GameEvent> GetUpcomingEvents() =>
        _events
            .Where(e => e.StartsAt >= DateTime.Now)
            .OrderBy(e => e.StartsAt);

    public IEnumerable<GameEvent> GetEventsWithAvailableSeats() =>
        GetUpcomingEvents().Where(e => e.HasAvailableSeats);

    public IEnumerable<BoardGame> GetSuitableAvailableGames(GameEvent gameEvent) =>
        _games
            .Where(g => g.Availability == GameAvailability.Available)
            .Where(g => g.SupportsPlayerCount(Math.Max(1, gameEvent.ParticipantCount)))
            .OrderBy(g => g.Title);
}
