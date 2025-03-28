using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp2.Shared.Classes;

namespace WpfApp2.Views.StamData.Countries.UI;

public partial class CountriesList
{
    // --- Dependency Properties
    public static readonly DependencyProperty CountriesObsProperty = DependencyProperty.Register(
        nameof(CountriesObs), typeof(IObservable<IEnumerable<Country>>), typeof(CountriesList),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not CountriesList c) return;
            c.Disposables.Add(c.CountriesObs.Subscribe(countries => { c.Countries = new ObservableCollection<Country>(countries); }));
        }));

    public IObservable<IEnumerable<Country>> CountriesObs
    {
        get => (IObservable<IEnumerable<Country>>) GetValue(CountriesObsProperty);
        set => SetValue(CountriesObsProperty, value);
    }

    // --- Events
    public event Action<Country>? Selected;
    public event Action? Add;
    public event Action? Reload;

    // --- Internal Properties
    [ObservableProperty] private ObservableCollection<Country>? _countries;

    // --- Constructor
    public CountriesList()
    {
        InitializeComponent();

        ListView.SelectionChanged += (_, _) =>
        {
            if (ListView.SelectedItem is Country selected)
            {
                Selected?.Invoke(selected);
            }
        };
    }

    private void New_Click(object sender, RoutedEventArgs e) => Add?.Invoke();

    private void Refresh_Click(object sender, RoutedEventArgs e) => Reload?.Invoke();
}