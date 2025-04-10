using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp2.Data.Classes;

namespace WpfApp2.Views.StamData.Warehouses.UI;

public partial class WarehousesList
{
    // --- Dependency Properties
    public static readonly DependencyProperty WarehousesObsProperty = DependencyProperty.Register(
        nameof(WarehousesObs), typeof(IObservable<IEnumerable<Warehouse>>), typeof(WarehousesList),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not WarehousesList c) return;
            c.Disposables.Add(c.WarehousesObs.Subscribe(warehouses => { c.Warehouses = new ObservableCollection<Warehouse>(warehouses); }));
        }));

    public IObservable<IEnumerable<Warehouse>> WarehousesObs
    {
        get => (IObservable<IEnumerable<Warehouse>>) GetValue(WarehousesObsProperty);
        set => SetValue(WarehousesObsProperty, value);
    }

    // --- Events
    public event Action<Warehouse>? Selected;
    public event Action? Add;
    public event Action? Reload;

    // --- Internal Properties
    [ObservableProperty] private ObservableCollection<Warehouse>? _warehouses;

    // --- Constructor
    public WarehousesList()
    {
        InitializeComponent();

        ListView.SelectionChanged += (_, _) =>
        {
            if (ListView.SelectedItem is Warehouse selected)
            {
                Selected?.Invoke(selected);
            }
        };
    }

    private void New_Click(object sender, RoutedEventArgs e) => Add?.Invoke();

    private void Refresh_Click(object sender, RoutedEventArgs e) => Reload?.Invoke();
}