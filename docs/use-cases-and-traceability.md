# Use cases och spårbarhet

## Identifierade use cases

1. Administratören registrerar en ny medlem.
2. Administratören registrerar ett nytt spel i spelbiblioteket.
3. Administratören skapar en ny spelträff.
4. En medlem anmäls till eller avanmäls från en spelträff.
5. Arrangören reserverar ett spel för en spelträff.
6. Användaren söker, filtrerar och sorterar information.

## UC1 - Registrera ny medlem

**Aktör:** Administratör  
**Mål:** Lägga till en ny aktiv medlem i föreningens medlemslista.  
**Förutsättning:** Programmet är startat.

### Huvudflöde

1. Administratören öppnar fliken Medlemmar.
2. Administratören skriver medlemmens namn och e-postadress.
3. Administratören klickar på Lägg till.
4. Systemet kontrollerar att namn och e-post är giltiga.
5. Systemet kontrollerar att e-postadressen inte redan används.
6. Systemet skapar medlemmen med ett nytt medlemsnummer och aktiv status.
7. Den uppdaterade medlemslistan visas.

### Alternativt flöde

- Om namn saknas, e-post är ogiltig eller adressen redan används visas ett felmeddelande.
- Ingen medlem sparas när kontrollen misslyckas.

## UC2 - Anmäla medlem till spelträff

**Aktör:** Medlem eller arrangör  
**Mål:** Koppla en aktiv medlem till en kommande spelträff.  
**Förutsättning:** Medlemmen och spelträffen finns registrerade.

### Huvudflöde

1. Användaren öppnar fliken Spelträffar.
2. Användaren väljer en spelträff.
3. Användaren väljer en aktiv medlem.
4. Användaren klickar på Anmäl.
5. Systemet kontrollerar att medlemmen inte redan är anmäld.
6. Systemet kontrollerar att träffen har en ledig plats.
7. Systemet skapar en anmälan och visar medlemmen i deltagarlistan.
8. Antalet anmälda och lediga platser uppdateras.

### Alternativa flöden

- Om medlemmen redan är anmäld stoppas anmälan och ett tydligt meddelande visas.
- Om träffen är fullbokad stoppas anmälan och ett tydligt meddelande visas.
- Om medlemmen är inaktiv stoppas anmälan.

## UC3 - Skapa spelträff

**Aktör:** Administratör  
**Mål:** Skapa en öppen spelkväll, temakväll eller turnering.

Kort flöde: Administratören anger namn, typ, datum, tid, plats, maxantal deltagare och en aktiv
arrangör. Vid en temakväll anges tema och vid en turnering anges vilket spel turneringen gäller.
Systemet validerar informationen och skapar rätt underklass till `GameEvent`.

## UC4 - Reservera spel för spelträff

**Aktör:** Arrangör  
**Mål:** Planera vilket spel som ska användas vid en träff.

Kort flöde: Arrangören väljer träff och ett tillgängligt spel. Systemet kopplar spelet till träffen och
ändrar dess status till reserverat. Samma spel kan därför inte reserveras för två träffar samtidigt.

## Spårbarhet

| Behov eller regel | Use case | Domänklass/metod | Visas i gränssnittet |
| --- | --- | --- | --- |
| Registrera och uppdatera medlemmar | UC1 | `Member`, `ClubManager.AddMember` | Fliken Medlemmar |
| Ingen dubblerad e-post | UC1 | `ClubManager.AddMember` | Felmeddelande |
| Skapa olika aktiviteter | UC3 | `GameEvent` och dess underklasser | Formuläret Skapa spelträff |
| Stoppa dubbelanmälan | UC2 | `GameEvent.Register` | Felmeddelande |
| Stoppa anmälan när träffen är full | UC2 | `GameEvent.Register` | Felmeddelande |
| Visa deltagare och lediga platser | UC2 | `Registrations`, `RemainingSeats` | Träfflista och deltagarlista |
| Reservera spel för aktivitet | UC4 | `GameEvent.AddPlannedGame`, `BoardGame.ReserveFor` | Planerade spel |
| Söka och filtrera | UC1, UC6 | `SearchMembers`, `SearchGames` | Sökfält och kategorifilter |
| Sortera kommande träffar | UC6 | `GetUpcomingEvents` | Översikt och träfflista |
| Gruppera spel efter kategori | UC6 | `GetGamesGroupedByCategory` | Text under spelbiblioteket |
