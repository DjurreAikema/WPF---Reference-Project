using System.Windows;
using WpfApp2.Shared.Navigation;
using WpfApp2.Views;

namespace WpfApp2;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly NavigationService _navigationService;

    public MainWindow()
    {
        InitializeComponent();

        _navigationService = new NavigationService(MainContentControl);

        Stamdata_Click(this, new RoutedEventArgs());
    }

    public NavigationService GetNavigationService() => _navigationService;

    private void Stamdata_Click(object sender, RoutedEventArgs e) =>
        _navigationService.NavigateTo(new StamdataView());

    private void Warehouses_Click(object sender, RoutedEventArgs e) =>
        _navigationService.NavigateTo(new WarehousesView());

    private void Snacks_Click(object sender, RoutedEventArgs e) =>
        _navigationService.NavigateTo(new SnacksView());
}