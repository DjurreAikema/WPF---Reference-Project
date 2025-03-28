using System.Windows;
using System.Windows.Controls;

namespace WpfApp2.Views;

public partial class UnitSizesManagementView : UserControl
{
    public UnitSizesManagementView()
    {
        InitializeComponent();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        // Navigate back to Stamdata
        var mainWindow = (MainWindow)Application.Current.MainWindow;
        var navigationService = mainWindow.GetNavigationService();
        navigationService.NavigateBack();
    }
}