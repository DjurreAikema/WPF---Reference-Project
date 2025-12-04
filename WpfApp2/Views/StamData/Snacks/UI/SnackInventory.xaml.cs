using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp2.Data.Classes.Stamdata;

namespace WpfApp2.Views.StamData.Snacks.UI;

public partial class SnackInventory
{
    // --- Dependency Properties
    public static readonly DependencyProperty InventoryStateObsProperty = DependencyProperty.Register(
        nameof(InventoryStateObs), typeof(IObservable<SnacksInventoryState>), typeof(SnackInventory),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not SnackInventory c) return;
            c.Disposables.Add(c.InventoryStateObs.Subscribe(state =>
            {
                c.InventoryEntries = new ObservableCollection<Inventory>(state.Inventory);
                c.UnitSizes = new ObservableCollection<UnitSize>(state.UnitSizes ?? []);
                c.HasMultipleUnitSizes = state.HasMultipleUnitSizes;
                c.SnackId = state.SnackId;

                c.UpdateColumns(state.HasMultipleUnitSizes);
                c.New();
            }));
        }));

    public IObservable<SnacksInventoryState> InventoryStateObs
    {
        get => (IObservable<SnacksInventoryState>) GetValue(InventoryStateObsProperty);
        set => SetValue(InventoryStateObsProperty, value);
    }

    // --- Events
    public event Action<Inventory>? Saved;
    public event Action<int>? Deleted;

    // --- Internal Properties
    [ObservableProperty] private ObservableCollection<Inventory>? _inventoryEntries;
    [ObservableProperty] private Inventory? _selected;

    [ObservableProperty] private ObservableCollection<UnitSize>? _unitSizes;

    [ObservableProperty] private bool _hasMultipleUnitSizes;
    [ObservableProperty] private int? _snackId;

    // --- Constructor
    public SnackInventory()
    {
        InitializeComponent();

        // Set up grid selection handler
        Dg.SelectionChanged += (_, _) =>
        {
            if (Dg.SelectedItem is not Inventory selected) return;
            Selected = selected;
        };
    }

    // --- Methods
    private void New()
    {
        if (!SnackId.HasValue) return;

        var newEntry = new Inventory
        {
            SnackId = SnackId.Value,
            WarehouseId = null
        };

        Selected = newEntry;
    }

    private void UpdateColumns(bool hasMultipleUnitSizes)
    {
        // First check if we already have the unit size column
        var unitSizeColumn = Dg.Columns.FirstOrDefault(col =>
            col is DataGridTextColumn dtc &&
            dtc.Header as string == "Unit Size");

        if (hasMultipleUnitSizes && unitSizeColumn == null)
        {
            // We need to add the column
            var newColumn = new DataGridTextColumn
            {
                Header = "Unit Size",
                Binding = new Binding("UnitSize.Name"),
                Width = new DataGridLength(20, DataGridLengthUnitType.Star)
            };

            // Insert before the Quantity column (assuming it's the last column)
            Dg.Columns.Insert(Dg.Columns.Count - 1, newColumn);
        }
        else if (!hasMultipleUnitSizes && unitSizeColumn != null)
        {
            // We need to remove the column
            Dg.Columns.Remove(unitSizeColumn);
        }
    }

    // --- Event Handlers
    private void New_Click(object sender, RoutedEventArgs e) => New();

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
}