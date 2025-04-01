using System.Collections.ObjectModel;
using WpfApp2.Shared.Navigation;
using WpfApp2.Shared.UI.Navigation;
using WpfApp2.Views;
using WpfApp2.Views.StamData;

namespace WpfApp2;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly NavigationService _navigationService;

    private const string STAMDATA = "stamdata";
    private const string WAREHOUSES = "warehouses";
    private const string SNACKS = "snacks";

    public MainWindow()
    {
        InitializeComponent();

        _navigationService = new NavigationService(MainContentControl);

        // Initialize sidebar items
        var sidebarItems = new ObservableCollection<SidebarItem>
        {
            new SidebarItem
            {
                Text = "Stamdata",
                IconData = NavigationIcons.Database,
                Destination = STAMDATA
            },
            new SidebarItem
            {
                Text = "Warehouses",
                IconData = NavigationIcons.Warehouse,
                Destination = WAREHOUSES
            },
            new SidebarItem
            {
                Text = "Snacks",
                IconData = NavigationIcons.Snacks,
                Destination = SNACKS
            }
        };

        MainSidebar.SidebarItems = sidebarItems;
        MainSidebar.SelectedItem = STAMDATA;
        MainSidebar_NavigationRequested(this, STAMDATA);
    }

    public NavigationService GetNavigationService() => _navigationService;

    private void MainSidebar_NavigationRequested(object sender, string destination)
    {
        switch (destination)
        {
            case STAMDATA:
                _navigationService.NavigateTo(new StamdataView());
                break;
            case WAREHOUSES:
                _navigationService.NavigateTo(new WarehousesView());
                break;
            case SNACKS:
                _navigationService.NavigateTo(new SnacksView());
                break;
        }
    }
}