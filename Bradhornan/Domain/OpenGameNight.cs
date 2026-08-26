namespace Bradhornan.Domain;

public class OpenGameNight : GameEvent
{

    public override string EventType => "Öppen spelkväll";

    public OpenGameNight(string title, DateTime startsAt, string location, int capacity, Member organizer)
        : base(title, startsAt, location, capacity, organizer)
    {
    }

}
