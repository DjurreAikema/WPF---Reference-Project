using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp2.Data.Classes;

namespace WpfApp2.Views.StamData.Snacks.UI;

public partial class SnackInventory
{
    // --- Dependency Properties
    public static readonly DependencyProperty InventoryObsProperty = DependencyProperty.Register(
        nameof(InventoryObs), typeof(IObservable<ObservableCollection<Inventory>>), typeof(SnackInventory),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not SnackInventory c) return;
            c.Disposables.Add(c.InventoryObs.Subscribe(inventories => { c.InventoryEntries = inventories; }));
        }));

    public IObservable<ObservableCollection<Inventory>> InventoryObs
    {
        get => (IObservable<ObservableCollection<Inventory>>) GetValue(InventoryObsProperty);
        set => SetValue(InventoryObsProperty, value);
    }

    public static readonly DependencyProperty UnitSizesObsProperty = DependencyProperty.Register(
        nameof(UnitSizesObs), typeof(IObservable<ObservableCollection<UnitSize>>), typeof(SnackInventory),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not SnackInventory c) return;
            c.Disposables.Add(c.UnitSizesObs.Subscribe(unitSizes =>
            {
                c.UnitSizes = unitSizes;
                if (unitSizes != null && unitSizes.Count > 0)
                {
                    c.HasMultipleUnitSizes = true;
                    c.OnPropertyChanged(nameof(HasMultipleUnitSizes));
                }
            }));
        }));

    public IObservable<ObservableCollection<UnitSize>> UnitSizesObs
    {
        get => (IObservable<ObservableCollection<UnitSize>>) GetValue(UnitSizesObsProperty);
        set => SetValue(UnitSizesObsProperty, value);
    }

    public static readonly DependencyProperty SnackIdObsProperty = DependencyProperty.Register(
        nameof(SnackIdObs), typeof(IObservable<int?>), typeof(SnackInventory),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not SnackInventory c) return;
            c.Disposables.Add(c.SnackIdObs.Subscribe(id => { c.SnackId = id; }));
        }));

    public IObservable<int?> SnackIdObs
    {
        get => (IObservable<int?>) GetValue(SnackIdObsProperty);
        set => SetValue(SnackIdObsProperty, value);
    }

    // --- Events
    public event Action<Inventory>? Saved;
    public event Action<int>? Deleted;

    // --- Internal Properties
    [ObservableProperty] private ObservableCollection<Inventory>? _inventoryEntries;
    [ObservableProperty] private Inventory? _selected;

    [ObservableProperty] private ObservableCollection<UnitSize>? _unitSizes;
    [ObservableProperty] private UnitSize? _selectedUnitSize;

    [ObservableProperty] private int? _snackId;
    [ObservableProperty] private bool _hasMultipleUnitSizes;

    // --- Constructor
    public SnackInventory()
    {
        InitializeComponent();

        // Set up grid selection handler
        Dg.SelectionChanged += (_, _) =>
        {
            if (Dg.SelectedItem is not Inventory selected) return;
            Selected = selected;

            // When selecting an item, also select the corresponding unit size if applicable
            if (HasMultipleUnitSizes && UnitSizes != null)
            {
                SelectedUnitSize = UnitSizes.FirstOrDefault(u => u.Id == selected.UnitSizeId);
            }
        };
    }

    // --- Methods


    // --- Event Handlers
    private void New_Click(object sender, RoutedEventArgs e)
    {
        if (!SnackId.HasValue) return;

        var newEntry = new Inventory
        {
            SnackId = SnackId.Value
        };

        // If we have unit sizes and they're selected, associate with the selected unit size
        if (HasMultipleUnitSizes && SelectedUnitSize != null)
        {
            newEntry.UnitSizeId = SelectedUnitSize.Id;
        }

        Selected = newEntry;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (Selected == null || !SnackId.HasValue) return;
        Saved?.Invoke(Selected);
        Dg.SelectedItem = null;
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (Selected == null || Selected.Id is 0 or null) return;
        Deleted?.Invoke(Selected.Id.Value);
        Selected = null;
    }

    private void OnWarehouseSelected(int? warehouseId)
    {
        if (Selected != null) Selected.WarehouseId = warehouseId ?? 0;
    }

    private void OnUnitSizeChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (Selected == null || SelectedUnitSize == null) return;
        Selected.UnitSizeId = SelectedUnitSize.Id;
    }
}