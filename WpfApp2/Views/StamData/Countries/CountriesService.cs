using WpfApp2.Shared.Debugging.Extensions;

namespace WpfApp2.Views.StamData.Countries;

/// <summary>
/// Provides a singleton instance of the CountriesViewModel
/// for use throughout the application.
/// </summary>
public static class CountriesService
{
    private static readonly Lazy<CountriesViewModel> _instance = new(() => new CountriesViewModel()
        .RegisterWithTracker(isSingleton: true));

    /// <summary>
    /// Gets the singleton instance of the CountriesViewModel.
    /// </summary>
    public static CountriesViewModel Instance => _instance.Value;
}

// There's one potential concern to be aware of: since we're now using a singleton for the CountriesViewModel,
// the state will persist between different views that use it. This means:
// 1. If you navigate away from the Countries view and come back, it will still have the same selected country
// 2. If you open the Countries view in a standalone window, it will share state with the main window