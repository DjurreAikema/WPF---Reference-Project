using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp2.Data.Classes;

namespace WpfApp2.Views.StamData.Countries.FormUI;

public partial class CountriesComboBox
{
    // --- Dependency Properties
    public static readonly DependencyProperty SelectedCountryIdProperty = DependencyProperty.Register(
        nameof(SelectedCountryId), typeof(int?), typeof(CountriesComboBox),
        new PropertyMetadata(null, OnSelectedCountryIdChanged));

    public int? SelectedCountryId
    {
        get => (int?)GetValue(SelectedCountryIdProperty);
        set => SetValue(SelectedCountryIdProperty, value);
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
            Countries = new ObservableCollection<Country>(countries);
            UpdateSelectedCountryFromId();
        }));

        // Subscribe to the loading state
        Disposables.Add(CountriesService.Instance.LoadingObs.Subscribe(loading =>
        {
            IsLoading = loading;
        }));

        // Trigger a reload of countries just to be sure we have the latest data
        CountriesService.Instance.Reload.OnNext(new System.Reactive.Unit());
    }

    // --- Methods
    private void UpdateSelectedCountryFromId()
    {
        if (SelectedCountryId.HasValue && Countries != null && Countries.Any())
        {
            var country = Countries.FirstOrDefault(c => c.Id == SelectedCountryId);
            if (country != null) Selected = country;
        }
        else if (!SelectedCountryId.HasValue)
        {
            Selected = null;
        }
    }

    private static void OnSelectedCountryIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CountriesComboBox selector) selector.UpdateSelectedCountryFromId();
    }

    private void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        SelectedCountryId = Selected?.Id;
        SelectedEvent?.Invoke(SelectedCountryId);
    }
}