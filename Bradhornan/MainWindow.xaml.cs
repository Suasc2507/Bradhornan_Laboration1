using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Bradhornan.Domain;
using Bradhornan.Services;

namespace Bradhornan;

public partial class MainWindow : Window
{
    private readonly ClubManager _manager = DemoData.Create();

    public MainWindow()
    {
        InitializeComponent();

        GameCategoryComboBox.ItemsSource = Enum.GetValues<GameCategory>();
        GameDifficultyComboBox.ItemsSource = Enum.GetValues<DifficultyLevel>();

        var categoryFilterItems = new List<object> { "Alla kategorier" };
        categoryFilterItems.AddRange(Enum.GetValues<GameCategory>().Cast<object>());
        GameCategoryFilterComboBox.ItemsSource = categoryFilterItems;

        EventTypeComboBox.ItemsSource = new[] { "Öppen spelkväll", "Temakväll", "Turnering" };

        GameCategoryComboBox.SelectedIndex = 0;
        GameDifficultyComboBox.SelectedIndex = 0;
        GameCategoryFilterComboBox.SelectedIndex = 0;
        EventTypeComboBox.SelectedIndex = 0;
        EventDatePicker.SelectedDate = DateTime.Today.AddDays(7);
        EventTimeTextBox.Text = "18:00";
        EventCapacityTextBox.Text = "10";
        EventLocationTextBox.Text = "Föreningslokalen";
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) => RefreshAll();

    private void RefreshAll()
    {
        RefreshMembers();
        RefreshGames();
        RefreshEvents();

        MemberCountText.Text = _manager.Members.Count.ToString();
        GameCountText.Text = _manager.Games.Count.ToString();
        EventCountText.Text = _manager.GetUpcomingEvents().Count().ToString();
        AvailableEventCountText.Text = _manager.GetEventsWithAvailableSeats().Count().ToString();
        UpcomingEventGrid.ItemsSource = _manager.GetUpcomingEvents().ToList();

        List<Member> activeMembers = _manager.Members.Where(m => m.IsActive).OrderBy(m => m.Name).ToList();
        EventOrganizerComboBox.ItemsSource = activeMembers;
        RegistrationMemberComboBox.ItemsSource = activeMembers;
    }

    private void RefreshMembers()
    {
        MemberGrid.ItemsSource = _manager.SearchMembers(MemberSearchTextBox.Text).ToList();
    }

    private void RefreshGames()
    {
        GameCategory? selectedCategory = GameCategoryFilterComboBox.SelectedItem is GameCategory category
            ? category
            : null;

        GameGrid.ItemsSource = _manager.SearchGames(GameSearchTextBox.Text, selectedCategory).ToList();

        GameGroupingText.Text = "Gruppering med LINQ: " + string.Join(" | ",
            _manager.GetGamesGroupedByCategory()
                .Select(group => $"{group.Key}: {group.Count()} spel"));
    }

    private void RefreshEvents(GameEvent? selectedEvent = null)
    {
        EventGrid.ItemsSource = _manager.GetUpcomingEvents().ToList();
        if (selectedEvent is not null)
            EventGrid.SelectedItem = selectedEvent;

        RefreshSelectedEventDetails();
    }

    private void RefreshSelectedEventDetails()
    {
        if (EventGrid.SelectedItem is not GameEvent gameEvent)
        {
            ParticipantListBox.ItemsSource = null;
            PlannedGamesListBox.ItemsSource = null;
            PlanGameComboBox.ItemsSource = null;
            SuitableGamesText.Text = "Välj en spelträff för att se deltagare och spel.";
            return;
        }

        ParticipantListBox.ItemsSource = gameEvent.Registrations.Select(r => r.Member).ToList();
        PlannedGamesListBox.ItemsSource = gameEvent.PlannedGames.ToList();
        PlanGameComboBox.ItemsSource = _manager.Games
            .Where(g => g.Availability == GameAvailability.Available)
            .OrderBy(g => g.Title)
            .ToList();

        List<BoardGame> suitableGames = _manager.GetSuitableAvailableGames(gameEvent).ToList();
        SuitableGamesText.Text = suitableGames.Count == 0
            ? "Inga tillgängliga spel passar det nuvarande deltagarantalet."
            : "Passar deltagarantalet: " + string.Join(", ", suitableGames.Select(g => g.Title));
    }

    private void MemberSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (IsLoaded)
            RefreshMembers();
    }

    private void MemberGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MemberGrid.SelectedItem is not Member member)
            return;

        MemberNameTextBox.Text = member.Name;
        MemberEmailTextBox.Text = member.Email;
    }

    private void AddMemberButton_Click(object sender, RoutedEventArgs e) => ExecuteAction(() =>
    {
        Member member = _manager.AddMember(MemberNameTextBox.Text, MemberEmailTextBox.Text);
        ClearMemberForm();
        RefreshAll();
        StatusTextBlock.Text = $"Medlemmen {member.Name} registrerades.";
    });

    private void UpdateMemberButton_Click(object sender, RoutedEventArgs e) => ExecuteAction(() =>
    {
        Member member = GetSelectedMember();
        _manager.UpdateMember(member, MemberNameTextBox.Text, MemberEmailTextBox.Text);
        RefreshAll();
        MemberGrid.SelectedItem = member;
        StatusTextBlock.Text = $"Uppgifterna för {member.Name} uppdaterades.";
    });

    private void ToggleMemberButton_Click(object sender, RoutedEventArgs e) => ExecuteAction(() =>
    {
        Member member = GetSelectedMember();
        member.SetActiveStatus(!member.IsActive);
        RefreshAll();
        MemberGrid.SelectedItem = member;
        StatusTextBlock.Text = $"{member.Name} är nu {member.StatusText.ToLowerInvariant()}.";
    });

    private void DeleteMemberButton_Click(object sender, RoutedEventArgs e) => ExecuteAction(() =>
    {
        Member member = GetSelectedMember();
        _manager.RemoveMember(member);
        ClearMemberForm();
        RefreshAll();
        StatusTextBlock.Text = $"{member.Name} togs bort.";
    });

    private Member GetSelectedMember() =>
        MemberGrid.SelectedItem as Member
        ?? throw new InvalidOperationException("Välj först en medlem i listan.");

    private void ClearMemberForm()
    {
        MemberNameTextBox.Clear();
        MemberEmailTextBox.Clear();
    }

    private void GameSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (IsLoaded)
            RefreshGames();
    }

    private void GameCategoryFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (IsLoaded)
            RefreshGames();
    }

    private void GameGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (GameGrid.SelectedItem is not BoardGame game)
            return;

        GameTitleTextBox.Text = game.Title;
        GameCategoryComboBox.SelectedItem = game.Category;
        GameMinPlayersTextBox.Text = game.MinPlayers.ToString();
        GameMaxPlayersTextBox.Text = game.MaxPlayers.ToString();
        GameMinutesTextBox.Text = game.PlayTimeMinutes.ToString();
        GameDifficultyComboBox.SelectedItem = game.Difficulty;
        GameDescriptionTextBox.Text = game.Description;
    }

    private void AddGameButton_Click(object sender, RoutedEventArgs e) => ExecuteAction(() =>
    {
        BoardGame game = _manager.AddGame(
            GameTitleTextBox.Text,
            GetSelectedCategory(),
            ParsePositiveInteger(GameMinPlayersTextBox.Text, "Minsta antal spelare"),
            ParsePositiveInteger(GameMaxPlayersTextBox.Text, "Högsta antal spelare"),
            ParsePositiveInteger(GameMinutesTextBox.Text, "Speltid"),
            GetSelectedDifficulty(),
            GameDescriptionTextBox.Text);

        ClearGameForm();
        RefreshAll();
        StatusTextBlock.Text = $"Spelet {game.Title} lades till.";
    });

    private void UpdateGameButton_Click(object sender, RoutedEventArgs e) => ExecuteAction(() =>
    {
        BoardGame game = GetSelectedGame();
        _manager.UpdateGame(
            game,
            GameTitleTextBox.Text,
            GetSelectedCategory(),
            ParsePositiveInteger(GameMinPlayersTextBox.Text, "Minsta antal spelare"),
            ParsePositiveInteger(GameMaxPlayersTextBox.Text, "Högsta antal spelare"),
            ParsePositiveInteger(GameMinutesTextBox.Text, "Speltid"),
            GetSelectedDifficulty(),
            GameDescriptionTextBox.Text);

        RefreshAll();
        GameGrid.SelectedItem = game;
        StatusTextBlock.Text = $"Spelet {game.Title} uppdaterades.";
    });

    private void DeleteGameButton_Click(object sender, RoutedEventArgs e) => ExecuteAction(() =>
    {
        BoardGame game = GetSelectedGame();
        _manager.RemoveGame(game);
        ClearGameForm();
        RefreshAll();
        StatusTextBlock.Text = $"Spelet {game.Title} togs bort.";
    });

    private BoardGame GetSelectedGame() =>
        GameGrid.SelectedItem as BoardGame
        ?? throw new InvalidOperationException("Välj först ett spel i listan.");

    private GameCategory GetSelectedCategory() =>
        GameCategoryComboBox.SelectedItem is GameCategory category
            ? category
            : throw new InvalidOperationException("Välj en kategori.");

    private DifficultyLevel GetSelectedDifficulty() =>
        GameDifficultyComboBox.SelectedItem is DifficultyLevel difficulty
            ? difficulty
            : throw new InvalidOperationException("Välj en svårighetsgrad.");

    private void ClearGameForm()
    {
        GameTitleTextBox.Clear();
        GameMinPlayersTextBox.Clear();
        GameMaxPlayersTextBox.Clear();
        GameMinutesTextBox.Clear();
        GameDescriptionTextBox.Clear();
        GameCategoryComboBox.SelectedIndex = 0;
        GameDifficultyComboBox.SelectedIndex = 0;
    }

    private void EventTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (EventExtraLabel is null || EventExtraTextBox is null)
            return;

        string selectedType = EventTypeComboBox.SelectedItem?.ToString() ?? "Öppen spelkväll";
        bool needsExtra = selectedType != "Öppen spelkväll";
        EventExtraLabel.Visibility = needsExtra ? Visibility.Visible : Visibility.Collapsed;
        EventExtraTextBox.Visibility = needsExtra ? Visibility.Visible : Visibility.Collapsed;
        EventExtraLabel.Text = selectedType == "Turnering" ? "Turneringsspel" : "Tema";
    }

    private void EventGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
        RefreshSelectedEventDetails();

    private void AddEventButton_Click(object sender, RoutedEventArgs e) => ExecuteAction(() =>
    {
        if (EventDatePicker.SelectedDate is not DateTime selectedDate)
            throw new InvalidOperationException("Välj ett datum.");

        if (!TimeSpan.TryParseExact(
                EventTimeTextBox.Text.Trim(),
                new[] { @"h\:mm", @"hh\:mm" },
                CultureInfo.InvariantCulture,
                out TimeSpan selectedTime))
            throw new InvalidOperationException("Tiden ska skrivas som HH:mm, till exempel 18:00.");

        Member organizer = EventOrganizerComboBox.SelectedItem as Member
            ?? throw new InvalidOperationException("Välj en arrangör.");

        GameEvent gameEvent = _manager.AddEvent(
            EventTitleTextBox.Text,
            selectedDate.Date.Add(selectedTime),
            EventLocationTextBox.Text,
            ParsePositiveInteger(EventCapacityTextBox.Text, "Max deltagare"),
            organizer,
            EventTypeComboBox.SelectedItem?.ToString() ?? "Öppen spelkväll",
            EventExtraTextBox.Text);

        RefreshAll();
        EventGrid.SelectedItem = gameEvent;
        StatusTextBlock.Text = $"Spelträffen {gameEvent.Title} skapades.";
    });

    private void DeleteEventButton_Click(object sender, RoutedEventArgs e) => ExecuteAction(() =>
    {
        GameEvent gameEvent = GetSelectedEvent();
        _manager.RemoveEvent(gameEvent);
        RefreshAll();
        StatusTextBlock.Text = $"Spelträffen {gameEvent.Title} togs bort.";
    });

    private void RegisterMemberButton_Click(object sender, RoutedEventArgs e) => ExecuteAction(() =>
    {
        GameEvent gameEvent = GetSelectedEvent();
        Member member = RegistrationMemberComboBox.SelectedItem as Member
            ?? throw new InvalidOperationException("Välj en medlem att anmäla.");

        gameEvent.Register(member);
        RefreshAll();
        EventGrid.SelectedItem = gameEvent;
        StatusTextBlock.Text = $"{member.Name} anmäldes till {gameEvent.Title}.";
    });

    private void CancelRegistrationButton_Click(object sender, RoutedEventArgs e) => ExecuteAction(() =>
    {
        GameEvent gameEvent = GetSelectedEvent();
        Member member = ParticipantListBox.SelectedItem as Member
            ?? throw new InvalidOperationException("Välj en deltagare i listan.");

        gameEvent.CancelRegistration(member);
        RefreshAll();
        EventGrid.SelectedItem = gameEvent;
        StatusTextBlock.Text = $"{member.Name} avanmäldes från {gameEvent.Title}.";
    });

    private void PlanGameButton_Click(object sender, RoutedEventArgs e) => ExecuteAction(() =>
    {
        GameEvent gameEvent = GetSelectedEvent();
        BoardGame game = PlanGameComboBox.SelectedItem as BoardGame
            ?? throw new InvalidOperationException("Välj ett spel att planera.");

        gameEvent.AddPlannedGame(game);
        RefreshAll();
        EventGrid.SelectedItem = gameEvent;
        StatusTextBlock.Text = $"{game.Title} reserverades för {gameEvent.Title}.";
    });

    private void RemovePlannedGameButton_Click(object sender, RoutedEventArgs e) => ExecuteAction(() =>
    {
        GameEvent gameEvent = GetSelectedEvent();
        BoardGame game = PlannedGamesListBox.SelectedItem as BoardGame
            ?? throw new InvalidOperationException("Välj ett planerat spel i listan.");

        gameEvent.RemovePlannedGame(game);
        RefreshAll();
        EventGrid.SelectedItem = gameEvent;
        StatusTextBlock.Text = $"{game.Title} togs bort från {gameEvent.Title}.";
    });

    private GameEvent GetSelectedEvent() =>
        EventGrid.SelectedItem as GameEvent
        ?? throw new InvalidOperationException("Välj först en spelträff i listan.");

    private static int ParsePositiveInteger(string text, string fieldName)
    {
        if (!int.TryParse(text, out int value) || value <= 0)
            throw new ArgumentException($"{fieldName} måste vara ett heltal större än noll.");

        return value;
    }

    private void ExecuteAction(Action action)
    {
        try
        {
            action();
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            StatusTextBlock.Text = exception.Message;
            MessageBox.Show(exception.Message, "Kontrollera uppgifterna", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
