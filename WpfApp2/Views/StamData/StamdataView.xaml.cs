using System.Windows;
using WpfApp2.Shared.Navigation;
using WpfApp2.Views.StamData.Countries;
using WpfApp2.Views.StamData.Snacks;
using WpfApp2.Views.StamData.Warehouses;

namespace WpfApp2.Views.StamData;

public partial class StamdataView
{
    public StamdataView()
    {
        InitializeComponent();
    }

    // --- Countries
    private void Countries_Click(object sender, RoutedEventArgs e)
    {
        // Get the NavigationService from MainWindow
        var mainWindow = (MainWindow) Application.Current.MainWindow;
        var navigationService = mainWindow.GetNavigationService();

        // Navigate to Countries Management view
        navigationService.NavigateTo(new CountriesView());
    }

    private void CountriesPopup_Click(object sender, RoutedEventArgs e)
    {
        // Create a new instance of the CountriesView
        var view = new CountriesView();

        // Create and show a new window containing the view
        var window = WindowFactory.CreateWindow(view, "Countries Management", 900, 600);
        window.Show();
    }

    // --- Snacks
    private void Snacks_Click(object sender, RoutedEventArgs e)
    {
        var mainWindow = (MainWindow) Application.Current.MainWindow;
        var navigationService = mainWindow.GetNavigationService();

        navigationService.NavigateTo(new SnacksView());
    }

    private void SnacksPopup_Click(object sender, RoutedEventArgs e)
    {
        var view = new SnacksView();
        var window = WindowFactory.CreateWindow(view, "Snacks Management", 900, 600);
        window.Show();
    }

    // --- Warehouses
    private void Warehouses_Click(object sender, RoutedEventArgs e)
    {
        var mainWindow = (MainWindow) Application.Current.MainWindow;
        var navigationService = mainWindow.GetNavigationService();

        navigationService.NavigateTo(new WarehousesView());
    }

    private void WarehousesPopup_Click(object sender, RoutedEventArgs e)
    {
        var view = new WarehousesView();
        var window = WindowFactory.CreateWindow(view, "Warehouses Management", 900, 600);
        window.Show();
    }
}