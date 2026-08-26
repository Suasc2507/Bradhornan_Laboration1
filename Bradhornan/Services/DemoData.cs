using Bradhornan.Domain;

namespace Bradhornan.Services;

public static class DemoData
{
    public static ClubManager Create()
    {
        var manager = new ClubManager();

        Member sara = manager.AddMember("Sara Lind", "sara@bradhornan.se", DateTime.Today.AddYears(-2));
        Member ali = manager.AddMember("Ali Hassan", "ali@bradhornan.se", DateTime.Today.AddMonths(-8));
        Member emma = manager.AddMember("Emma Berg", "emma@bradhornan.se", DateTime.Today.AddMonths(-5));
        Member oskar = manager.AddMember("Oskar Holm", "oskar@bradhornan.se", DateTime.Today.AddMonths(-2));
        manager.AddMember("Lina Ek", "lina@bradhornan.se", DateTime.Today.AddDays(-20));

        BoardGame catan = manager.AddGame(
            "Catan", GameCategory.Strategy, 3, 4, 90, DifficultyLevel.Intermediate,
            "Bygg vägar och samhällen på ön Catan.");
        BoardGame pandemic = manager.AddGame(
            "Pandemic", GameCategory.Cooperative, 2, 4, 60, DifficultyLevel.Intermediate,
            "Spelarna samarbetar för att stoppa sjukdomsutbrott.");
        manager.AddGame("Ticket to Ride", GameCategory.Family, 2, 5, 60, DifficultyLevel.Beginner);
        manager.AddGame("Codenames", GameCategory.Party, 4, 8, 30, DifficultyLevel.Beginner);
        manager.AddGame("Azul", GameCategory.Strategy, 2, 4, 45, DifficultyLevel.Intermediate);
        manager.AddGame("The Crew", GameCategory.Card, 3, 5, 20, DifficultyLevel.Intermediate);

        GameEvent openNight = manager.AddEvent(
            "Fredagsspel", DateTime.Today.AddDays(7).AddHours(18), "Föreningslokalen", 8,
            sara, "Öppen spelkväll", "");
        openNight.Register(ali);
        openNight.Register(emma);
        openNight.AddPlannedGame(catan);

        GameEvent themeNight = manager.AddEvent(
            "Samarbetskväll", DateTime.Today.AddDays(14).AddHours(18), "Föreningslokalen", 6,
            ali, "Temakväll", "Samarbetsspel");
        themeNight.Register(sara);
        themeNight.Register(oskar);
        themeNight.AddPlannedGame(pandemic);

        manager.AddEvent(
            "Catanmästerskapet", DateTime.Today.AddDays(21).AddHours(13), "Stora salen", 12,
            sara, "Turnering", "Catan");

        return manager;
    }
}
