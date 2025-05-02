using System.Collections.ObjectModel;
using System.Windows;

namespace WpfApp2.Shared.Navigation.UI;

public partial class Breadcrumb
{
    // --- Dependencies
    private NavigationService? _navigationService;

    // --- Properties
    public ObservableCollection<string> NavigationPath { get; } = [];

    // --- Constructor
    public Breadcrumb()
    {
        InitializeComponent();

        // Try to get the navigation service from the main window
        Loaded += (_, _) =>
        {
            if (Application.Current?.MainWindow is not MainWindow mainWindow) return;

            _navigationService = mainWindow.GetNavigationService();
            if (_navigationService == null) return;

            // Subscribe to navigation changes
            _navigationService.NavigationChanged += OnNavigationChanged;
            UpdateNavigationPath();
        };

        // Unsubscribe when unloaded
        Unloaded += (_, _) =>
        {
            if (_navigationService == null) return;
            _navigationService.NavigationChanged -= OnNavigationChanged;
        };
    }

    // --- Methods
    private void OnNavigationChanged(object? sender, EventArgs e) => UpdateNavigationPath();

    private void UpdateNavigationPath()
    {
        NavigationPath.Clear();

        if (_navigationService != null)
        {
            foreach (var item in _navigationService.GetNavigationPath())
            {
                NavigationPath.Add(item);
            }
        }

        BackButton.IsEnabled = _navigationService?.NavigationStack.Count > 1;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e) => _navigationService?.NavigateBack();
}