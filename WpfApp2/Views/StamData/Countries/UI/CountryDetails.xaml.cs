using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp2.Shared.Classes;

namespace WpfApp2.Views.StamData.Countries.UI;

public partial class CountryDetails
{
    // --- Dependency Properties
    public static readonly DependencyProperty SelectedObsProperty = DependencyProperty.Register(
        nameof(SelectedObs), typeof(IObservable<Country?>), typeof(CountryDetails),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not CountryDetails c) return;
            c.Disposables.Add(c.SelectedObs.Subscribe(obj =>
            {
                c.Selected = obj != null ? new Country(obj) : null;
                c.OnPropertyChanged(nameof(Selected));
            }));
        }));

    public IObservable<Country?> SelectedObs
    {
        get => (IObservable<Country?>) GetValue(SelectedObsProperty);
        set => SetValue(SelectedObsProperty, value);
    }

    // --- Events
    public event Action<Country>? Saved;
    public event Action<int>? Deleted;

    // --- Internal Properties
    [ObservableProperty] private Country? _selected;
    [ObservableProperty] private bool _hasId;

    // --- Constructor
    public CountryDetails()
    {
        InitializeComponent();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (Selected == null) return;
        Saved?.Invoke(Selected);
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (Selected == null || Selected.Id is 0 or null) return;
        Deleted?.Invoke(Selected.Id.Value);
    }
}