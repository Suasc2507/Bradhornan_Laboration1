# Brädhörnan - Laboration 1

Brädhörnan är ett enkelt WPF-program för en brädspelsförening. Programmet håller ordning på
medlemmar, spel och kommande spelträffar. Projektet är gjort för Laboration 1 i kursen
Objektorienterad programmering i C#.

## Så startar du programmet

1. Öppna `Bradhornan.sln` i Visual Studio 2022.
2. Kontrollera att arbetsbelastningen **.NET desktop development** är installerad.
3. Högerklicka på projektet `Bradhornan` och välj **Set as Startup Project** om det behövs.
4. Tryck på den gröna startknappen eller `F5`.

Projektet använder .NET 8 och WPF. Ingen databas behövs. Demonstrationsdata skapas automatiskt
varje gång programmet startas.

## Funktioner

- registrera, uppdatera, inaktivera och ta bort medlemmar
- registrera, uppdatera, söka, filtrera och ta bort spel
- skapa och ta bort öppna spelkvällar, temakvällar och turneringar
- anmäla och avanmäla medlemmar till en spelträff
- stoppa dubbelanmälningar och anmälningar till fullbokade träffar
- reservera spel för en viss träff
- visa spel som passar det aktuella antalet deltagare
- visa kommande träffar sorterade efter datum
- gruppera spel efter kategori med LINQ
- visa tydliga felmeddelanden vid ogiltig inmatning

## Förslag på demonstration

1. Visa översikten och de förifyllda träffarna.
2. Lägg till en medlem och sök sedan efter medlemmen.
3. Filtrera spelbiblioteket på en kategori och visa LINQ-grupperingen under listan.
4. Välj `Fredagsspel` och anmäl en medlem.
5. Försök anmäla samma medlem igen för att visa att dubbelanmälning stoppas.
6. Skapa en temakväll och välj en arrangör.
7. Reservera ett tillgängligt spel för träffen.

## Projektstruktur

- `Domain/` innehåller domänklasser och verksamhetsregler.
- `Services/ClubManager.cs` samordnar samlingarna och innehåller LINQ-frågorna.
- `Services/DemoData.cs` skapar demonstrationsdata.
- `MainWindow.xaml` och `MainWindow.xaml.cs` innehåller det enkla WPF-gränssnittet.
- `docs/` innehåller use cases, designreflektion och UML-diagram.

## Dokumentation

- [Use cases och spårbarhet](docs/use-cases-and-traceability.md)
- [Designreflektion](docs/design-reflection.md)
- [Klassdiagram](docs/class-diagram.md)
- [Sekvensdiagram](docs/sequence-diagram.md)

## Inför inlämningen

Båda gruppmedlemmarna ska förstå lösningen och bidra aktivt i GitHub. Gör riktiga ändringar och
commits under arbetet, till exempel kod, testning, felrättningar och dokumentation. Ta bort mapparna
`bin` och `obj` innan projektet packas som ZIP.
