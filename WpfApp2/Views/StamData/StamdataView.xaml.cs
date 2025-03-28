using System.Windows;
using WpfApp2.Views.StamData.Countries;

namespace WpfApp2.Views.StamData;

public partial class StamdataView
{
    public StamdataView()
    {
        InitializeComponent();
    }

    private void ManageCountries_Click(object sender, RoutedEventArgs e)
    {
        // Get the NavigationService from MainWindow
        var mainWindow = (MainWindow) Application.Current.MainWindow;
        var navigationService = mainWindow.GetNavigationService();

        // Navigate to Countries Management view
        navigationService.NavigateTo(new CountriesView());
    }
}