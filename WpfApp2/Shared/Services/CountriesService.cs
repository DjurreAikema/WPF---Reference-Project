using WpfApp2.Views.StamData.Countries;

namespace WpfApp2.Shared.Services;

/// <summary>
/// Provides a singleton instance of the CountriesViewModel
/// for use throughout the application.
/// </summary>
public static class CountriesService
{
    private static readonly Lazy<CountriesViewModel> _instance =
        new Lazy<CountriesViewModel>(() => new CountriesViewModel());

    /// <summary>
    /// Gets the singleton instance of the CountriesViewModel.
    /// </summary>
    public static CountriesViewModel Instance => _instance.Value;
}