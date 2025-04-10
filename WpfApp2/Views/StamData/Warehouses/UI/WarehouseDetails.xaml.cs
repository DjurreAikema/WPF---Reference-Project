using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp2.Data.Classes;

namespace WpfApp2.Views.StamData.Warehouses.UI;

public partial class WarehouseDetails
{
    // --- Dependency Properties
    public static readonly DependencyProperty SelectedObsProperty = DependencyProperty.Register(
        nameof(SelectedObs), typeof(IObservable<Warehouse?>), typeof(WarehouseDetails),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not WarehouseDetails c) return;
            c.Disposables.Add(c.SelectedObs.Subscribe(obj =>
            {
                c.Selected = obj != null ? new Warehouse(obj) : null;
                c.OnPropertyChanged(nameof(Selected));
            }));
        }));

    public IObservable<Warehouse?> SelectedObs
    {
        get => (IObservable<Warehouse?>) GetValue(SelectedObsProperty);
        set => SetValue(SelectedObsProperty, value);
    }

    // --- Events
    public event Action<Warehouse>? Saved;
    public event Action<int>? Deleted;

    // --- Internal Properties
    [ObservableProperty] private Warehouse? _selected;
    [ObservableProperty] private bool _hasId;

    // --- Constructor
    public WarehouseDetails()
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

    private void OnCountrySelected(int? countryId)
    {
        if (Selected != null) Selected.CountryId = countryId;
    }
}