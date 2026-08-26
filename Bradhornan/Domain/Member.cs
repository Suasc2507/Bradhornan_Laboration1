namespace Bradhornan.Domain;


public class Member
{
    public Guid Id { get; } = Guid.NewGuid();
    public int MemberNumber { get; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime JoinedOn { get; }

    public string StatusText => IsActive ? "Aktiv" : "Inaktiv";

    public Member(int memberNumber, string name, string email, DateTime joinedOn)
    {
        if (memberNumber <= 0)
            throw new ArgumentException("Medlemsnumret måste vara större än noll.");

        MemberNumber = memberNumber;
        JoinedOn = joinedOn.Date;
        IsActive = true;
        UpdateContactDetails(name, email);
    }

    public void UpdateContactDetails(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Medlemmens namn måste anges.");

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("Ange en giltig e-postadress.");

        Name = name.Trim();
        Email = email.Trim();
    }

    public void SetActiveStatus(bool isActive) => IsActive = isActive;

    public override string ToString() => $"{MemberNumber} - {Name}";

}
