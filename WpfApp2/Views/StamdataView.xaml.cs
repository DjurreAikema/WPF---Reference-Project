using System.Windows;
using System.Windows.Controls;
using WpfApp2.Views.StamData;
using WpfApp2.Views.StamData.UnitSize;

namespace WpfApp2.Views;

public partial class StamdataView : UserControl
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
        navigationService.NavigateTo(new CountriesManagementView());
    }

    private void ManageUnitSizes_Click(object sender, RoutedEventArgs e)
    {
        // Get the NavigationService from MainWindow
        var mainWindow = (MainWindow) Application.Current.MainWindow;
        var navigationService = mainWindow.GetNavigationService();

        // Navigate to Unit Sizes Management view
        // navigationService.NavigateTo(new UnitSizesManagementView());
    }
}