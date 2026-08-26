# Designreflektion

Vi valde att avgränsa första versionen till föreningens tre huvudområden: medlemmar, spelbibliotek
och spelträffar. De centrala klasserna blev därför `Member`, `BoardGame`, `GameEvent` och
`Registration`. `ClubManager` håller ihop samlingarna och används av gränssnittet för att utföra
övergripande operationer och sökningar.

Verksamhetsreglerna ligger så nära objekten som möjligt. `GameEvent` ansvarar till exempel för att
kontrollera dubbelanmälningar, lediga platser och vilka spel som reserverats. `BoardGame` ansvarar för
sin tillgänglighet och reservation. Detta gör att samma regler gäller även om gränssnittet senare byts
ut eller byggs om i Laboration 2.

Vi använder arv och polymorfism för aktiviteter. `GameEvent` är abstrakt och har gemensamma
egenskaper som datum, plats, arrangör och deltagare. `OpenGameNight`, `ThemeNight` och `Tournament`
ärver från klassen och ger olika värden på `EventType`. Gränssnittet kan därför hantera alla typer genom
typen `GameEvent`, samtidigt som varje underklass har sin egen information.

Samlingsklassen `List<T>` används för medlemmar, spel, träffar, anmälningar och planerade spel.
Listorna exponeras som skrivskyddade vyer så att förändringar går genom avsedda metoder. LINQ används
bland annat för att filtrera aktiva medlemmar och sökresultat, sortera spel och kommande träffar samt
gruppera spel efter kategori. En mer avancerad fråga kombinerar flera villkor för att hitta tillgängliga
spel som passar antalet deltagare på en vald träff.

Felaktiga situationer hanteras med `ArgumentException` och `InvalidOperationException` i domän- och
serviceklasserna. WPF-fönstret fångar dessa förväntade fel och visar begripliga meddelanden. Programmet
kraschar därför inte vid normala användarfel, exempelvis ogiltig e-post, dubbelanmälan eller fullbokad
träff.

Gränssnittet är medvetet enkelt och använder code-behind. Full MVVM ingår inte i kraven för
Laboration 1. Domänklasserna är ändå separerade från WPF-koden, vilket gör det möjligt att behålla
kärnlogiken och införa MVVM, databas och fler lager i Laboration 2.

Demonstrationsdata skapas i `DemoData`. Det gör att programmet direkt kan visa sökning, sortering,
deltagarlistor och spelreservationer utan mycket manuell förberedelse. Betalningar, autentisering,
e-post och andra externa tjänster har avgränsats bort enligt verksamhetsbeskrivningen.
