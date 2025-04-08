using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp2.Data.Classes;

namespace WpfApp2.Views.Snacks.UI;

public class InventoryEntry
{
    public int? Id { get; set; }
    public int SnackId { get; set; }
    public int WarehouseId { get; set; }
    public int? UnitSizeId { get; set; }
    public string UnitSizeName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.Now;

    // Navigation properties
    public Warehouse? Warehouse { get; set; }
    public UnitSize? UnitSize { get; set; }

    public InventoryEntry()
    {
    }

    public InventoryEntry(Inventory inventory, string unitSizeName = "Default")
    {
        Id = inventory.Id;
        SnackId = inventory.SnackId;
        WarehouseId = inventory.WarehouseId;
        UnitSizeId = inventory.UnitSizeId;
        Quantity = inventory.Quantity;
        Warehouse = inventory.Warehouse;
        UnitSize = inventory.UnitSize;
        UnitSizeName = inventory.UnitSize?.Name ?? unitSizeName;
        LastUpdated = DateTime.Now;
    }

    // Convert back to Inventory for saving
    public Inventory ToInventory()
    {
        return new Inventory
        {
            Id = Id,
            SnackId = SnackId,
            WarehouseId = WarehouseId,
            UnitSizeId = UnitSizeId,
            Quantity = Quantity
        };
    }
}

public partial class SnackInventory
{
    // --- Dependency Properties
    public static readonly DependencyProperty InventoryObsProperty = DependencyProperty.Register(
        nameof(InventoryObs), typeof(IObservable<ObservableCollection<Inventory>>), typeof(SnackInventory),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not SnackInventory c) return;
            c.Disposables.Add(c.InventoryObs.Subscribe(inventories => { c.ProcessInventoryData(inventories); }));
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
    [ObservableProperty] private ObservableCollection<InventoryEntry>? _inventoryEntries;
    [ObservableProperty] private InventoryEntry? _selected;
    [ObservableProperty] private ObservableCollection<UnitSize>? _unitSizes;
    [ObservableProperty] private UnitSize? _selectedUnitSize;
    [ObservableProperty] private int? _snackId;
    [ObservableProperty] private bool _hasMultipleUnitSizes;

    // --- Constructor
    public SnackInventory()
    {
        InitializeComponent();

        // Initialize collections
        InventoryEntries = [];

        // Set up grid selection handler
        Dg.SelectionChanged += (_, _) =>
        {
            if (Dg.SelectedItem is not InventoryEntry selected) return;
            Selected = selected;

            // When selecting an item, also select the corresponding unit size if applicable
            if (HasMultipleUnitSizes && UnitSizes != null)
            {
                SelectedUnitSize = UnitSizes.FirstOrDefault(u => u.Name == selected.UnitSizeName);
            }
        };
    }

    // --- Methods
    private void ProcessInventoryData(ObservableCollection<Inventory> inventories)
    {
        if (inventories == null || inventories.Count == 0)
        {
            InventoryEntries = [];
            return;
        }

        var entries = new ObservableCollection<InventoryEntry>();

        // If we don't have multiple unit sizes, just create entries directly from inventories
        if (!HasMultipleUnitSizes)
        {
            foreach (var inventory in inventories)
            {
                entries.Add(new InventoryEntry(inventory));
            }
        }
        else
        {
            // For multiple unit sizes, we need to connect inventory items with unit sizes
            // This is a simplification - in a real app, you might have a direct relationship in the database
            foreach (var inventory in inventories)
            {
                // For this example, we'll just associate with the first unit size
                // You would need to modify your data model to properly track which inventory belongs to which unit size
                string unitSizeName = "Default";
                if (UnitSizes != null && UnitSizes.Count > 0)
                {
                    var unitSize = UnitSizes.FirstOrDefault();
                    if (unitSize != null)
                    {
                        unitSizeName = unitSize.Name;
                    }
                }

                entries.Add(new InventoryEntry(inventory, unitSizeName));
            }
        }

        InventoryEntries = entries;
    }

    // --- UI Event Handlers
    private void New_Click(object sender, RoutedEventArgs e)
    {
        if (!SnackId.HasValue) return;

        var newEntry = new InventoryEntry
        {
            SnackId = SnackId.Value
        };

        // If we have unit sizes and they're selected, associate with the selected unit size
        if (HasMultipleUnitSizes && SelectedUnitSize != null)
        {
            newEntry.UnitSizeId = SelectedUnitSize.Id;
            newEntry.UnitSizeName = SelectedUnitSize.Name;
        }

        Selected = newEntry;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (Selected == null || !SnackId.HasValue) return;

        var inventory = Selected.ToInventory();
        Saved?.Invoke(inventory);
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
        Selected.UnitSizeName = SelectedUnitSize.Name;
    }
}