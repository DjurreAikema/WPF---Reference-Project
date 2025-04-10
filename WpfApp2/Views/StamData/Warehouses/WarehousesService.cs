namespace WpfApp2.Views.StamData.Warehouses;

/// <summary>
/// Provides a singleton instance of the WarehousesViewModel
/// for use throughout the application.
/// </summary>
public class WarehousesService
{
    private static readonly Lazy<WarehousesViewModel> _instance =
        new Lazy<WarehousesViewModel>(() => new WarehousesViewModel());

    /// <summary>
    /// Gets the singleton instance of the WarehousesViewModel.
    /// </summary>
    public static WarehousesViewModel Instance => _instance.Value;
}

// There's one potential concern to be aware of: since we're now using a singleton for the WarehousesViewModel,
// the state will persist between different views that use it. This means:
// 1. If you navigate away from the Warehouses view and come back, it will still have the same selected warehouse
// 2. If you open the Warehouses view in a standalone window, it will share state with the main window