namespace Bradhornan.Domain;

public class ThemeNight : GameEvent
{
    public string Theme { get; }
    public override string EventType => $"Temakväll: {Theme}";

    public ThemeNight(
        string title,
        DateTime startsAt,
        string location,
        int capacity,
        Member organizer,
        string theme)
        : base(title, startsAt, location, capacity, organizer)
    {
        if (string.IsNullOrWhiteSpace(theme))
            throw new ArgumentException("Ett tema måste anges för en temakväll.");

        Theme = theme.Trim();
    }
}
