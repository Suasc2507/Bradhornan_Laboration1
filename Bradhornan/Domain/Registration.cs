namespace Bradhornan.Domain;

public class Registration
{
    public Member Member { get; }
    public DateTime RegisteredAt { get; }

    public Registration(Member member)
    {
        Member = member ?? throw new ArgumentNullException(nameof(member));
        RegisteredAt = DateTime.Now;
    }

}
