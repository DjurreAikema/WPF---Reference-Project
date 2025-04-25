using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp2.Data.Classes;

namespace WpfApp2.Views.StamData.Warehouses.FormUI;

public partial class WarehousesComboBox
{
    private const int PlaceholderId = -999;

    // --- Dependency Properties
    public static readonly DependencyProperty SelectedIdProperty = DependencyProperty.Register(
        nameof(SelectedId), typeof(int?), typeof(WarehousesComboBox),
        new PropertyMetadata(null, OnSelectedIdChanged));

    public int? SelectedId
    {
        get => (int?) GetValue(SelectedIdProperty);
        set => SetValue(SelectedIdProperty, value);
    }

    private static void OnSelectedIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WarehousesComboBox selector) selector.UpdateSelectedFromId();
    }

    // --- Events
    public event Action<int?>? SelectedEvent;

    // --- Internal Properties
    [ObservableProperty] private ObservableCollection<Warehouse>? _warehouses = [];
    [ObservableProperty] private Warehouse? _selected;
    [ObservableProperty] private bool _isLoading;

    public WarehousesComboBox()
    {
        InitializeComponent();

        // Subscribe to the countries observable from the singleton ViewModel
        Disposables.Add(WarehousesService.Instance.WarehousesObs.Subscribe(warehouses =>
        {
            var warehousesList = new ObservableCollection<Warehouse>(warehouses);

            // Add placeholder item if the collection is empty
            if (!warehousesList.Any())
            {
                warehousesList.Add(new Warehouse
                {
                    Id = PlaceholderId,
                    Name = "No warehouses found"
                });
            }

            Warehouses = warehousesList;
            UpdateSelectedFromId();
        }));

        // Subscribe to the loading state
        Disposables.Add(WarehousesService.Instance.LoadingObs.Subscribe(loading => { IsLoading = loading; }));

        // Trigger a reload of warehouses just to be sure we have the latest data
        WarehousesService.Instance.Reload.OnNext(new System.Reactive.Unit());
    }

    // --- Methods
    private void UpdateSelectedFromId()
    {
        if (SelectedId.HasValue && Warehouses != null && Warehouses.Any())
        {
            var warehouse = Warehouses.FirstOrDefault(c => c.Id == SelectedId);
            if (warehouse != null) Selected = warehouse;
        }
        else if (!SelectedId.HasValue)
        {
            Selected = null;
        }
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