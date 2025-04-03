using System.Collections.ObjectModel;
using WpfApp2.Shared.Navigation;
using WpfApp2.Shared.Navigation.UI;
using WpfApp2.Views;
using WpfApp2.Views.Snacks;
using WpfApp2.Views.StamData;
using WpfApp2.Views.Warehouses;

namespace WpfApp2;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly NavigationService _navigationService;

    public MainWindow()
    {
        InitializeComponent();

        _navigationService = new NavigationService(MainContentControl);

        MainSidebar.SidebarItems = InitializeSidebarItems();
        MainSidebar.SelectedItem = Stamdata;
        OnNavigationRequested(this, Stamdata);
    }

    public NavigationService GetNavigationService() => _navigationService;

    // --- Sidebar navigation

    #region Sidebar navigation

    private const string Stamdata = "stamdata";
    private const string Warehouses = "warehouses";
    private const string Snacks = "snacks";

    private static ObservableCollection<SidebarItem> InitializeSidebarItems()
    {
        return
        [
            new SidebarItem
            {
                Text = "Stamdata",
                IconData = NavigationIcons.Database,
                Destination = Stamdata
            },
            new SidebarItem
            {
                Text = "Warehouses",
                IconData = NavigationIcons.Warehouse,
                Destination = Warehouses
            },
            new SidebarItem
            {
                Text = "Snacks",
                IconData = NavigationIcons.Snacks,
                Destination = Snacks
            }
        ];
    }

    private void OnNavigationRequested(object sender, string destination)
    {
        switch (destination)
        {
            case Stamdata:
                _navigationService.NavigateTo(new StamdataView());
                break;
            case Warehouses:
                _navigationService.NavigateTo(new WarehousesView());
                break;
            case Snacks:
                _navigationService.NavigateTo(new SnacksView());
                break;
        }
    }

    #endregion
}