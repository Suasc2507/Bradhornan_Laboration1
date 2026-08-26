namespace Bradhornan.Domain;

public abstract class GameEvent
{
    private readonly List<Registration> _registrations = new();
    private readonly List<BoardGame> _plannedGames = new();

    public Guid Id { get; } = Guid.NewGuid();
    public string Title { get; private set; } = string.Empty;
    public DateTime StartsAt { get; private set; }
    public string Location { get; private set; } = string.Empty;
    public int Capacity { get; private set; }
    public Member Organizer { get; private set; } = null!;
    public abstract string EventType { get; }

    public IReadOnlyList<Registration> Registrations => _registrations.AsReadOnly();
    public IReadOnlyList<BoardGame> PlannedGames => _plannedGames.AsReadOnly();
    public int ParticipantCount => _registrations.Count;
    public int RemainingSeats => Capacity - ParticipantCount;
    public bool HasAvailableSeats => RemainingSeats > 0;
    public string DateText => StartsAt.ToString("yyyy-MM-dd HH:mm");

    protected GameEvent(string title, DateTime startsAt, string location, int capacity, Member organizer)
    {
        UpdateDetails(title, startsAt, location, capacity, organizer);
    }

    public void UpdateDetails(string title, DateTime startsAt, string location, int capacity, Member organizer)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Spelträffens namn måste anges.");

        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Spelträffens plats måste anges.");

        if (capacity < 1)
            throw new ArgumentException("Maxantalet deltagare måste vara minst 1.");

        if (capacity < ParticipantCount)
            throw new InvalidOperationException("Maxantalet kan inte vara lägre än antalet anmälda.");

        if (organizer is null || !organizer.IsActive)
            throw new ArgumentException("Arrangören måste vara en aktiv medlem.");

        Title = title.Trim();
        StartsAt = startsAt;
        Location = location.Trim();
        Capacity = capacity;
        Organizer = organizer;
    }

    public void Register(Member member)
    {
        if (!member.IsActive)
            throw new InvalidOperationException("Endast aktiva medlemmar kan anmäla sig.");

        if (_registrations.Any(r => r.Member.Id == member.Id))
            throw new InvalidOperationException("Medlemmen är redan anmäld till denna spelträff.");

        if (!HasAvailableSeats)
            throw new InvalidOperationException("Spelträffen är fullbokad.");

        _registrations.Add(new Registration(member));
    }

    public void CancelRegistration(Member member)
    {
        Registration? registration = _registrations.FirstOrDefault(r => r.Member.Id == member.Id);
        if (registration is null)
            throw new InvalidOperationException("Medlemmen är inte anmäld till denna spelträff.");

        _registrations.Remove(registration);
    }

    public void AddPlannedGame(BoardGame game)
    {
        if (_plannedGames.Any(g => g.Id == game.Id))
            throw new InvalidOperationException("Spelet är redan planerat för denna spelträff.");

        game.ReserveFor(this);
        _plannedGames.Add(game);
    }

    public void RemovePlannedGame(BoardGame game)
    {
        if (!_plannedGames.Remove(game))
            throw new InvalidOperationException("Spelet är inte planerat för denna spelträff.");

        game.ReleaseReservation(Id);
    }

    public override string ToString() => $"{Title} - {DateText}";
}
