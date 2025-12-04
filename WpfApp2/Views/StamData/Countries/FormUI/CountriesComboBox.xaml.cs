using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp2.Data.Classes.Stamdata;
using WpfApp2.Shared.Navigation;

namespace WpfApp2.Views.StamData.Countries.FormUI;

public partial class CountriesComboBox
{
    private const int PlaceholderId = -999;

    // --- Dependency Properties
    public static readonly DependencyProperty SelectedIdProperty = DependencyProperty.Register(
        nameof(SelectedId), typeof(int?), typeof(CountriesComboBox),
        new PropertyMetadata(null, OnSelectedIdChanged));

    public int? SelectedId
    {
        get => (int?) GetValue(SelectedIdProperty);
        set => SetValue(SelectedIdProperty, value);
    }

    // --- Events
    public event Action<int?>? SelectedEvent;

    // --- Internal Properties
    [ObservableProperty] private ObservableCollection<Country>? _countries = [];
    [ObservableProperty] private Country? _selected;
    [ObservableProperty] private bool _isLoading;

    public CountriesComboBox()
    {
        InitializeComponent();

        // Subscribe to the countries observable from the singleton ViewModel
        Disposables.Add(CountriesService.Instance.CountriesObs.Subscribe(countries =>
        {
            var countriesList = new ObservableCollection<Country>(countries);

            // Add placeholder item if the collection is empty
            if (!countriesList.Any())
            {
                countriesList.Add(new Country
                {
                    Id = PlaceholderId,
                    Name = "No countries found"
                });
            }

            Countries = countriesList;
            UpdateSelectedFromId();
        }));

        // Subscribe to the loading state
        Disposables.Add(CountriesService.Instance.LoadingObs.Subscribe(loading => { IsLoading = loading; }));

        // Trigger a reload of countries just to be sure we have the latest data
        CountriesService.Instance.Reload.OnNext(new System.Reactive.Unit());
    }

    // --- Methods
    private void UpdateSelectedFromId()
    {
        if (SelectedId.HasValue && Countries != null && Countries.Any())
        {
            var country = Countries.FirstOrDefault(c => c.Id == SelectedId);
            if (country != null) Selected = country;
        }
        else if (!SelectedId.HasValue)
        {
            Selected = null;
        }
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var isControlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

        if (isControlPressed)
        {
            // Open in a new window if Control is pressed
            var view = new CountriesView();
            var window = WindowFactory.CreateWindow(view, "Countries Management", 900, 600);
            window.Show();
        }
        else
        {
            // Navigate in the main content area if Control is not pressed
            var mainWindow = (MainWindow) Application.Current.MainWindow;
            var navigationService = mainWindow.GetNavigationService();

            var countriesView = new CountriesView();
            navigationService.NavigateTo(countriesView);
        }
    }

    private static void OnSelectedIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CountriesComboBox selector) selector.UpdateSelectedFromId();
    }

    private void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        // Prevent selection of the placeholder item
        if (Selected?.Id == PlaceholderId)
        {
            Selected = null;
            return;
        }

        SelectedId = Selected?.Id;
        SelectedEvent?.Invoke(SelectedId);
    }
}