using System.Collections.ObjectModel;
using WpfApp2.Shared.Navigation;
using WpfApp2.Shared.Navigation.UI;
using WpfApp2.Views.Debug;
using WpfApp2.Views.StamData;

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
    private const string Debug = "debug";

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
                Text = "Debug Tools",
                IconData = NavigationIcons.Settings,
                Destination = Debug
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
            case Debug:
                _navigationService.NavigateTo(new DebugView());
                break;
        }
    }

    #endregion
}